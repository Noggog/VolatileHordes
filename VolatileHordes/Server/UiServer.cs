using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UniLinq;
using VolatileHordes.Allocation;
using VolatileHordes.Dto;
using VolatileHordes.Players;
using VolatileHordes.Tracking;

namespace VolatileHordes.Server
{
    public class UiServer
    {
        private readonly LimitManager _limitManager;
        private readonly PlayerZoneManager _players;
        private readonly AllocationBuckets _allocationBuckets;
        private readonly ZombieGroupManager _zombies;

        public class Client
        {
            public Socket sock = null!;
            public bool disconnected;
        }

        private Socket _listener = new(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        private List<Client> _clients = new();

        private Subject<Unit> _clientEventOccurred = new();

        private AllocationStateDto _allocationState = null!;

        public UiServer(
            TimeManager timeManager,
            UiSettings settings,
            LimitManager limitManager,
            PlayerZoneManager players,
            AllocationBuckets allocationBuckets,
            ZombieGroupManager zombies)
        {
            _limitManager = limitManager;
            _players = players;
            _allocationBuckets = allocationBuckets;
            _zombies = zombies;
            IPEndPoint localEndPoint = new(IPAddress.Any, settings.ViewServerPort);
            Logger.Info("Server listening on port {0}", settings.ViewServerPort);

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(100);
                _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to start server: {0}", ex.Message);
            }

            Bootstrapper.GameStarted
                .Subscribe(_ =>
                {
                    _allocationState = new AllocationStateDto(allocationBuckets.Width, allocationBuckets.Height);
                });
            
            timeManager.Interval(TimeSpan.FromMilliseconds(250))
                .FlowSwitch(Bootstrapper.GameStarted
                    .Select(_ => true)
                    .StartWith(false))
                .FlowSwitch(
                    _clientEventOccurred
                        .Select(_ => _clients.Count > 0)
                        .DistinctUntilChanged()
                        .Do(_ =>
                        {
                            Logger.Info("Turning server {0}", _clients.Count > 0 ? "ON" : "OFF");
                        }))
                .Subscribe(Update);
        }

        public bool Start(int port)
        {
            IPEndPoint localEndPoint = new(IPAddress.Any, port);

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(100);
                _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to start server: {0}", ex.Message);
                return false;
            }

            return true;
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;

            try
            {
                var client = new Client();
                client.sock = listener.EndAccept(ar);
                client.disconnected = false;

                _clients.Add(client);

                Logger.Debug("Client connected {0}", client.sock.RemoteEndPoint);

                _clientEventOccurred.OnNext();

                // Accept next.
                _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to accept new client: {0}", ex.Message);
            }
        }

        public void Update()
        {
            try
            {
                RemoveClients();
                Broadcast(CreateState());
            }
            catch (Exception ex)
            {
                Logger.Error("UI server update caused exception {0}", ex);
            }
        }

        private void RemoveClients()
        {
            try
            {
                for (int i = _clients.Count - 1; i >= 0; i--)
                {
                    Client client = _clients[i];
                    if (!client.disconnected) continue;
                    
                    Logger.Debug("Client disconnected {0}", client.sock.RemoteEndPoint);
                    _clients.RemoveAt(i);
                    _clientEventOccurred.OnNext();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UI server remove clients caused exception {0}", ex);
            }
        }
        
        private void Send(Client client, byte[] data, int length)
        {
            if (client.disconnected)
                return;

            try
            {
                var sock = client.sock;
                sock.Send(data, 0, length, 0);
            }
            catch (SocketException)
            {
            }
            catch (Exception ex)
            {
                client.disconnected = true;
                Logger.Error("Client send bytes caused exception {0}", ex);
            }
        }

        public void Send(Client cl, MemoryStream stream)
        {
            try
            {
                Send(cl, stream.GetBuffer(), (int)stream.Length);
            }
            catch (Exception ex)
            {
                cl.disconnected = true;
                Logger.Error("Client send stream caused exception {0}", ex);
            }
        }

        private State CreateState()
        {
            for (int x = 0; x < _allocationState.Buckets.GetLength(0); x++)
            {
                for (int y = 0; y < _allocationState.Buckets.GetLength(1); y++)
                {
                    _allocationState.Buckets[x, y] = _allocationBuckets[x, y];
                }
            }
            
            return new State()
            {
                AllocationState = _allocationState,
                Limits = new ZombieLimitsDto()
                {
                    CurrentNumber = _limitManager.CurrentlyActiveZombies,
                    GameMaximum = _limitManager.GameMaximumZombies,
                    DesiredMaximum = _limitManager.DesiredMaximumZombies 
                },
                Players = _players.Zones.Select(p => new PlayerDto()
                {
                    SpawnRectangle = p.SpawnRectangle,
                    Rectangle = p.Rectangle,
                    Rotation = p.Rotation.y,
                    Name = p.Player.TryGetEntity()?.GetDebugName() ?? string.Empty,
                }).ToList(),
                ZombieGroups = _zombies.AllGroups.Select(g =>
                {
                    return new ZombieGroupDto()
                    {
                        Id = g.Id,
                        Target = g.Target ?? new PointF(),
                        Zombies = g.Zombies
                            .Where(z => z.IsAlive && !z.IsDespawned)
                            .Select(z =>
                            {
                                return new ZombieDto()
                                {
                                    EntityId = z.Id,
                                    Position = z.GetPosition() ?? new PointF(),
                                    Target = z.GetTarget() ?? new PointF(),
                                    Rotation = z.GetRotation()?.y ?? 0f,
                                };
                            }).ToList()
                    };
                }).ToList()
            };
        }

        public void Broadcast(State state)
        {
            if (_clients.Count == 0)
                return;

            var memStream = new MemoryStream();
            state.Serialize(new BinaryWriter(memStream));

            try
            {
                foreach (var client in _clients)
                {
                    Send(client, memStream);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Broadcast caused exception {0}", ex);
            }
        }
    }
}