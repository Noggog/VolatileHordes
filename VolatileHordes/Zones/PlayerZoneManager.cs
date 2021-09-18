using System;
using System.Collections.Generic;
using UnityEngine;

namespace VolatileHordes.Zones
{
    public class PlayerZoneManager
    {
        static int ChunkViewDim = GamePrefs.GetInt(EnumGamePrefs.ServerMaxAllowedViewDistance);

        static Vector3 ChunkSize = new(16, 256, 16);
        static Vector3 VisibleBox = ChunkSize * ChunkViewDim;
        static Vector3 SpawnBlockBox = new(VisibleBox.x - 32, VisibleBox.y - 32, VisibleBox.z - 32);

        protected readonly List<PlayerZone> _zones = new();
        public IReadOnlyList<PlayerZone> Zones => _zones;

        public PlayerZoneManager(TimeManager time)
        {
            Logger.Info("Player Chunk View Dim: {0} - {1} - {2}", ChunkViewDim,
                VisibleBox,
                SpawnBlockBox);
            time.UpdateTicks()
                .Subscribe(_ => Update());
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
                    case RespawnType.LoadedGame:
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
            foreach (var zone in _zones)
            {
                if (zone.EntityId == entityId)
                    return;
            }

            var area = new PlayerZone(entityId);
            
            _zones.Add(UpdatePlayer(area));

            Logger.Info("Added player {0}", entityId);
        }

        private void RemovePlayer(int entityId)
        {
            for (int i = 0; i < _zones.Count; i++)
            {
                var ply = _zones[i] as PlayerZone;
                if (ply.EntityId == entityId)
                {
                    Logger.Info("Removed player: {0}", entityId);
                    _zones.RemoveAt(i);
                    return;
                }
            }
        }

        PlayerZone UpdatePlayer(PlayerZone ply, EntityPlayer ent)
        {
            var pos = ent.GetPosition();
            ply.Mins = (pos - (VisibleBox * 0.5f)).ToPoint();
            ply.Maxs = (pos + (VisibleBox * 0.5f)).ToPoint();
            ply.MinsSpawnBlock = (pos - (SpawnBlockBox * 0.5f)).ToPoint();
            ply.MaxsSpawnBlock = (pos + (SpawnBlockBox * 0.5f)).ToPoint();
            ply.Center = pos.ToPoint();
            return ply;
        }

        PlayerZone UpdatePlayer(PlayerZone ply)
        {
            if (ply.TryGetPlayer(out var ent))
            {
                ply = UpdatePlayer(ply, ent);
            }

            return ply;
        }

        public void Update()
        {
            var world = GameManager.Instance.World;
            var players = world.Players.dict;

            for (int i = _zones.Count - 1; i >= 0; i--)
            {
                var ply = _zones[i];

                if (players.TryGetValue(ply.EntityId, out var ent))
                {
                    _zones[i] = UpdatePlayer(ply, ent);
                }
                else
                {
                    // Remove player.
                    ply.Valid = false;
                    _zones.RemoveAt(i);
                    i--;

                    Logger.Error("Player not in player list: {0}", ply.EntityId);
                }
            }
        }

        public bool HasPlayers() => _zones.Count > 0;

        public void Print()
        {
            foreach (var zone in _zones)
            {
                string playerPos;
                if (zone.TryGetPlayer(out var player))
                {
                    playerPos = player.GetPosition().ToString();
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