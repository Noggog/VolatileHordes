using System;
using System.Drawing;
using UniLinq;
using UnityEngine;
using VolatileHordes.Allocation;
using VolatileHordes.Core.Services;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Players;
using VolatileHordes.Probability;
using VolatileHordes.Utility;

namespace VolatileHordes.Spawning
{
    public class SpawningPositions
    {
        private readonly IWorld _world;
        private readonly PlayerZoneManager _playerZoneManager;
        private readonly IChunkMeasurements _chunkMeasurements;
        private readonly RandomSource _randomSource;

        public SpawningPositions(
            IWorld world,
            PlayerZoneManager playerZoneManager,
            IChunkMeasurements chunkMeasurements,
            RandomSource randomSource)
        {
            _world = world;
            _playerZoneManager = playerZoneManager;
            _chunkMeasurements = chunkMeasurements;
            _randomSource = randomSource;
        }

        public PlayerSpawn? GetRandomTarget(bool nearPlayer)
        {
            if (nearPlayer)
            {
                return GetRandomNearPlayer();
            }
            else
            {
                return GetRandomTarget();
            }
        }

        public PlayerSpawn? GetRandomTarget()
        {
            var zone = GetRandomPlayerZone();
            if (zone == null) return null;
            var pos = GetRandomSafeCorner(zone);
            if (pos == null) return null;
            return new PlayerSpawn(pos.Value, zone);
        }

        public Vector3? GetRandomLocationInChunk(Point chunkPoint)
        {
            var bounds = _chunkMeasurements.GetBucketBounds(chunkPoint);
            Logger.Info("Bounds were {0}", bounds);
            return GetRandomPosition(bounds);
        }

        public SpawnTarget? GetRandomSpawnInChunk(Point chunkPoint)
        {
            var zones = _playerZoneManager.Zones
                .Where(z => _chunkMeasurements.GetAllocationBucket(z.Center) == chunkPoint)
                .ToArray();
            if (zones.Length == 0)
                return default;

            var zone = zones.Random(_randomSource);
            if (zone == null) return null;
            var pos = GetRandomSafeCorner(zone);
            if (pos == null) return null;
            var target = GetRandomLocationInChunk(chunkPoint);
            if (target == null) return null;
            return new SpawnTarget(pos.Value, target.Value.ToPoint(), zone);
        }

        public PlayerSpawn? GetRandomNearPlayer()
        {
            var spawnTarget = GetRandomTarget();
            if (spawnTarget == null) return null;
            var newTarget = GetRandomEdgeRangeAwayFrom(spawnTarget.Player.Center, range: 30);
            if (newTarget == null) return null;
            return new PlayerSpawn(
                newTarget.Value,
                spawnTarget.Player);
        }

        public PlayerZone? GetRandomPlayerZone()
        {
            if (_playerZoneManager.Zones.Count == 0)
                return default;

            var idx = _randomSource.Get(0, _playerZoneManager.Zones.Count);
            return _playerZoneManager.Zones[idx];
        }

        public Vector3? GetRandomPointNear(PointF pt, byte range, int attemptCount = 10)
        {
            var rect = new RectangleF(
                x: pt.X - range,
                y: pt.Y - range,
                width: range * 2,
                height: range * 2);
            return GetRandomPosition(rect, attemptCount);
        }

        public Vector3? GetRandomEdgeRangeAwayFrom(PointF pt, byte range)
        {
            var rect = new RectangleF(
                x: pt.X - range,
                y: pt.Y - range,
                width: range * 2,
                height: range * 2);
            
            foreach (var corner in rect.Corners()
                .ToArray()
                .EnumerateFromRandomIndex(_randomSource))
            {
                var worldPos = _world.GetWorldVector(corner);
                if (_world.CanSpawnAt(worldPos))
                {
                    return worldPos;
                }
            }

            return null;
        }
        
        public Vector3? GetRandomPosition(RectangleF zone, int attemptCount = 10)
        {
            for (int i = 0; i < attemptCount; i++)
            {
                var pos = TryGetSingleRandomZonePos(zone);
                var worldPos = _world.GetWorldVector(pos);
                if (_world.CanSpawnAt(worldPos))
                {
                    return worldPos;
                }
            }

            return null;
        }

        public Vector3? GetRandomSafeCorner(PlayerZone zone)
        {
            var corners = ZoneProcessing.EdgeCornersFromCluster(
                ZoneProcessing.GetConnectedSpawnRects(zone, _playerZoneManager.Zones)
                    .ToArray())
                .ToArray();

            foreach (var corner in corners.EnumerateFromRandomIndex(_randomSource))
            {
                var worldPos = _world.GetWorldVector(corner);
                if (_world.CanSpawnAt(worldPos))
                {
                    return worldPos;
                }
            }

            return null;
        }
        
        public PointF TryGetSingleRandomZonePos(RectangleF zone)
        {
            return new PointF(
                _randomSource.Get(zone.Left, zone.Right),
                _randomSource.Get(zone.Bottom, zone.Top));
        }

        public PlayerZone? GetNearestPlayer(PointF pt)
        {
            return _playerZoneManager.Zones
                .OrderBy(x => pt.AbsDistance(x.PlayerLocation))
                .FirstOrDefault();
        }
    }
}