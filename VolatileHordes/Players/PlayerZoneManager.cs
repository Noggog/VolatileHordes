using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UnityEngine;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Players
{
    public class PlayerZoneManager
    {
        private readonly IWorld _world;
        private readonly GameStageCalculator _gameStageCalculator;
        static int ChunkViewDim = GamePrefs.GetInt(EnumGamePrefs.ServerMaxAllowedViewDistance);

        static Vector3 ChunkSize = new(16, 256, 16);
        static Vector3 VisibleBox = ChunkSize * ChunkViewDim;
        static Vector3 SpawnBlockBox = new(VisibleBox.x - 32, VisibleBox.y - 32, VisibleBox.z - 32);

        public List<PlayerZone> Zones { get; } = new();

        private BehaviorSubject<int> _playerCount = new(0);
        public IObservable<int> PlayerCountObservable => _playerCount;

        // TODO: figure out easiest way to merge observables at initialization
        //private Subject<int> playerAdded = new();
        //private Subject<int> playerRemoved = new();
        //private Observable<List<int>> players =
        //    Observable.Merge(playerAdded, playerRemoved)

        public PlayerZoneManager(
            IWorld world,
            GameStageCalculator gameStageCalculator)
        {
            _world = world;
            _gameStageCalculator = gameStageCalculator;
            Bootstrapper.GameStarted.Subscribe(_ =>
            {
                Logger.Info("Player Chunk View Dim: {0} - {1} - {2}", ChunkViewDim,
                    VisibleBox,
                    SpawnBlockBox);
            });
        }

        public void PlayerSpawnedInWorld(ClientInfo? _cInfo, RespawnType _respawnReason, Vector3i _pos)
        {
            try
            {
                Logger.Debug("PlayerSpawnedInWorld \"{0}\", \"{1}\", \"{2}\"", _cInfo?.ToString() ?? "null", _respawnReason, _pos);
                int entityId = GetPlayerEntityId(_cInfo);
                switch (_respawnReason)
                {
                    case RespawnType.NewGame:
                    // case RespawnType.LoadedGame:
                    case RespawnType.EnterMultiplayer:
                    case RespawnType.JoinMultiplayer:
                        AddPlayer(entityId);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Error in API.PlayerSpawnedInWorld: {0}.", e.Message);
            }
        }

        public void PlayerDisconnected(ClientInfo? _cInfo, bool _bShutdown)
        {
            try
            {
                Logger.Debug("PlayerDisconnected \"{0}\", \"{1}\"", _cInfo?.ToString() ?? "null", _bShutdown);
                int entityId = GetPlayerEntityId(_cInfo);
                RemovePlayer(entityId);
            }
            catch (Exception e)
            {
                Logger.Error("Error in API.PlayerDisconnected: {0}.", e.Message);
            }
        }

        // Helper function for single player games where _cInfo is null.
        static int GetPlayerEntityId(ClientInfo? _cInfo)
        {
            if (_cInfo != null)
                return _cInfo.entityId;

            // On a local host this is set to null, grab id from player list.
            var player = GameManager.Instance.World.Players.list[0];

            return player.entityId;
        }
        
        private void AddPlayer(int entityId)
        {
            // This is called from PlayerSpawn, PlayerLogin has no entity id assigned yet.
            // So we have to check if the player is already here.
            foreach (var zone in Zones)
            {
                if (zone.Player.EntityId == entityId)
                    return;
            }

            var area = new PlayerZone(
                new PlayerGroup(_gameStageCalculator),
                new Player(_world, entityId));
            
            Zones.Add(UpdatePlayer(area));

            Logger.Info("Added player {0}", entityId);
            _playerCount.OnNext(_playerCount.Value + 1);
            //players.OnNext(entityId);
        }

        public PlayerZone UpdatePlayer(PlayerZone ply, EntityPlayer ent)
        {
            var pos = ent.GetPosition();
            ply.Mins = (pos - (VisibleBox * 0.5f)).ToPoint();
            ply.Maxs = (pos + (VisibleBox * 0.5f)).ToPoint();
            ply.MinsSpawnBlock = (pos - (SpawnBlockBox * 0.5f)).ToPoint();
            ply.MaxsSpawnBlock = (pos + (SpawnBlockBox * 0.5f)).ToPoint();
            ply.Center = pos.ToPoint();
            return ply;
        }

        public PlayerZone UpdatePlayer(PlayerZone ply)
        {
            if (ply.Player.TryGetEntity(out var ent))
            {
                ply = UpdatePlayer(ply, ent);
            }

            return ply;
        }

        private void RemovePlayer(int entityId)
        {
            for (int i = 0; i < Zones.Count; i++)
            {
                var ply = Zones[i] as PlayerZone;
                if (ply.Player.EntityId == entityId)
                {
                    Logger.Info("Removed player: {0}", entityId);
                    Zones.RemoveAt(i);
                    _playerCount.OnNext(_playerCount.Value - 1);
                    return;
                }
            }
        }

        public bool HasPlayers() => Zones.Count > 0;

        public void Print()
        {
            foreach (var zone in Zones)
            {
                string playerPos;
                if (zone.Player.TryGetEntity(out var entity))
                {
                    playerPos = entity.GetPosition().ToString();
                }
                else
                {
                    playerPos = "<MISSING>";
                }
                Logger.Info("Player zone center {0}.  Player currently at {1}", zone.Center, playerPos);
            }
        }
    }
}