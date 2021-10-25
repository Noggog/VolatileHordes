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
using VolatileHordes.Dto;
using VolatileHordes.Players;
using VolatileHordes.Tracking;

namespace VolatileHordes.Server
{
    public class UiServer
    {
        private readonly PlayerZoneManager _players;
        private readonly ZombieGroupManager _zombies;

        public class Client
        {
            public Socket sock = null!;
            public bool disconnected;
        }

        private Socket _listener = new(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        private List<Client> _clients = new();

        private Subject<Unit> _clientEventOccurred = new();

        public UiServer(
            TimeManager timeManager,
            UiSettings settings,
            PlayerZoneManager players,
            ZombieGroupManager zombies)
        {
            _players = players;
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

            timeManager.Interval(TimeSpan.FromMilliseconds(250))
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
            return new State()
            {
                Players = _players.Zones.Select(z => new PlayerDto()
                {
                    SpawnRectangle = z.SpawnRectangle,
                    Rectangle = z.Rectangle,
                    Rotation = z.Rotation.y
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