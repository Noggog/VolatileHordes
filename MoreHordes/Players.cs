using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreHordes
{
    public class PlayerZone : IZone
    {
        public bool valid = true;
        public int index = -1;
        public Vector3 mins = Vector3.zero;
        public Vector3 maxs = Vector3.zero;
        public Vector3 minsSpawnBlock = Vector3.zero;
        public Vector3 maxsSpawnBlock = Vector3.zero;
        public Vector3 center = Vector3.zero;
        public int entityId = -1;

        public int GetIndex()
        {
            return index;
        }

        // Zone AABB min
        public Vector3 GetMins()
        {
            return mins;
        }

        // Zone AABB min
        public Vector3 GetMaxs()
        {
            return maxs;
        }

        // Returns the center of the center.
        public Vector3 GetCenter()
        {
            return center;
        }

        // Returns a random position within the zone.
        public Vector3 GetRandomPos(RandomSource prng)
        {
            return new Vector3
            {
                x = prng.Get(mins.x, maxs.x),
                y = prng.Get(mins.y, maxs.y),
                z = prng.Get(mins.z, maxs.z),
            };
        }

        public bool IsInside2D(Vector3 pos)
        {
            return pos.x >= mins.x &&
                pos.z >= mins.z &&
                pos.x <= maxs.x &&
                pos.z <= maxs.z;
        }

        public bool InsideSpawnBlock2D(Vector3 pos)
        {
            return pos.x >= minsSpawnBlock.x &&
                pos.z >= minsSpawnBlock.z &&
                pos.x <= maxsSpawnBlock.x &&
                pos.z <= maxsSpawnBlock.z;
        }

        public bool InsideSpawnArea2D(Vector3 pos)
        {
            return IsInside2D(pos) && !InsideSpawnBlock2D(pos);
        }
        
        public Vector3? GetRandomZonePos()
        {
            var world = GameManager.Instance.World;

            Vector3 pos = new Vector3();
            Vector3 spawnPos = new Vector3();
            for (int i = 0; i < 10; i++)
            {
                pos.x = RandomSource.Instance.Get(minsSpawnBlock.x, maxsSpawnBlock.x);
                pos.z = RandomSource.Instance.Get(minsSpawnBlock.z, maxsSpawnBlock.z);

                int height = world.GetTerrainHeight((int)pos.x, (int)pos.z);

                spawnPos.x = pos.x;
                spawnPos.y = height + 1.0f;
                spawnPos.z = pos.z;
                if (world.CanMobsSpawnAtPos(spawnPos))
                {
                    return spawnPos;
                }
            }

            return null;
        }
    }

    public class PlayerZoneManager : ZoneManager<PlayerZone>
    {
        static int ChunkViewDim = GamePrefs.GetInt(EnumGamePrefs.ServerMaxAllowedViewDistance);

        static Vector3 ChunkSize = new Vector3(16, 256, 16);
        static Vector3 VisibleBox = ChunkSize * ChunkViewDim;
        static Vector3 SpawnBlockBox = new Vector3(VisibleBox.x - 32, VisibleBox.y - 32, VisibleBox.z - 32);

        public static readonly PlayerZoneManager Instance = new();
        
        private PlayerZoneManager()
        {
            Logger.Info("Player Chunk View Dim: {0} - {1} - {2}", ChunkViewDim,
                VisibleBox,
                SpawnBlockBox);
            TimeManager.Instance.UpdateTicks()
                .Subscribe(_ => Update());
        }

        public void PlayerSpawnedInWorld(ClientInfo _cInfo, RespawnType _respawnReason, Vector3i _pos)
        {
            try
            {
                Logger.Debug("PlayerSpawnedInWorld \"{0}\", \"{1}\", \"{2}\"", _cInfo, _respawnReason, _pos);
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

        public void PlayerDisconnected(ClientInfo _cInfo, bool _bShutdown)
        {
            try
            {
                Logger.Debug("PlayerDisconnected \"{0}\", \"{1}\"", _cInfo, _bShutdown);
                int entityId = GetPlayerEntityId(_cInfo);
                RemovePlayer(entityId);
            }
            catch (Exception e)
            {
                Logger.Error("Error in API.PlayerDisconnected: {0}.", e.Message);
            }
        }

        // Helper function for single player games where _cInfo is null.
        static int GetPlayerEntityId(ClientInfo _cInfo)
        {
            if (_cInfo != null)
                return _cInfo.entityId;

            // On a local host this is set to null, grab id from player list.
            var world = GameManager.Instance.World;
            var player = world.Players.list[0];

            return player.entityId;
        }
        
        private void AddPlayer(int entityId)
        {
            lock (_lock)
            {
                // This is called from PlayerSpawn, PlayerLogin has no entity id assigned yet.
                // So we have to check if the player is already here.
                foreach (var zone in _zones)
                {
                    if (zone.entityId == entityId)
                        return;
                }
                PlayerZone area = new PlayerZone
                {
                    index = _zones.Count,
                    entityId = entityId,
                };
                _zones.Add(UpdatePlayer(area, entityId));

                Logger.Info("Added player {0}", entityId);
            }
        }

        private void RemovePlayer(int entityId)
        {
            lock (_lock)
            {
                for (int i = 0; i < _zones.Count; i++)
                {
                    var ply = _zones[i] as PlayerZone;
                    if (ply.entityId == entityId)
                    {
                        Logger.Info("Removed player: {0}", entityId);
                        _zones.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        PlayerZone UpdatePlayer(PlayerZone ply, EntityPlayer ent)
        {
            var pos = ent.GetPosition();
            ply.mins = pos - (VisibleBox * 0.5f);
            ply.maxs = pos + (VisibleBox * 0.5f);
            ply.minsSpawnBlock = pos - (SpawnBlockBox * 0.5f);
            ply.maxsSpawnBlock = pos + (SpawnBlockBox * 0.5f);
            ply.center = pos;
            return ply;
        }

        PlayerZone UpdatePlayer(PlayerZone ply, int entityId)
        {
            var world = GameManager.Instance.World;
            var players = world.Players.dict;

            if (players.TryGetValue(entityId, out var ent))
            {
                ply = UpdatePlayer(ply, ent);
            }

            return ply;
        }

        public void Update()
        {
            lock (_lock)
            {
                var world = GameManager.Instance.World;
                var players = world.Players.dict;

                for (int i = 0; i < _zones.Count; i++)
                {
                    var ply = _zones[i];

                    if (players.TryGetValue(ply.entityId, out var ent))
                    {
                        _zones[i] = UpdatePlayer(ply, ent);
                    }
                    else
                    {
                        // Remove player.
                        ply.valid = false;
                        _zones.RemoveAt(i);
                        i--;

                        Logger.Error("Player not in player list: {0}", ply.entityId);
                    }

                    if (_zones.Count == 0)
                        break;
                }
            }
        }

        public bool HasPlayers()
        {
            lock (_lock)
            {
                return _zones.Count > 0;
            }
        }
    }
}