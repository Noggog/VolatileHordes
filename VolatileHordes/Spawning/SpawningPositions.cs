using System.Drawing;
using UniLinq;
using UnityEngine;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Randomization;
using VolatileHordes.Zones;

namespace VolatileHordes.Spawning
{
    public class SpawningPositions
    {
        private readonly IWorld _world;
        private readonly PlayerZoneManager _playerZoneManager;
        private readonly RandomSource _randomSource;

        public SpawningPositions(
            IWorld world,
            PlayerZoneManager playerZoneManager,
            RandomSource randomSource)
        {
            _world = world;
            _playerZoneManager = playerZoneManager;
            _randomSource = randomSource;
        }

        public SpawnTarget? GetRandomTarget()
        {
            var zone = GetRandomZone();
            if (zone == null) return null;
            Logger.Info("Player rect is {0}", zone.SpawnRectangle);
            var pos = GetRandomSafeCorner(zone);
            if (pos == null) return null;
            return new SpawnTarget(pos.Value, zone.Center);
        }

        public PlayerZone? GetRandomZone()
        {
            if (_playerZoneManager.Zones.Count == 0)
                return default;

            for (int i = 0; i < 4; i++)
            {
                var idx = _randomSource.Get(0, _playerZoneManager.Zones.Count);
                var zone = _playerZoneManager.Zones[idx];
                if (zone.Valid)
                {
                    return zone;
                }
            }

            return default;
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
        
        public Vector3? GetRandomPosition(RectangleF zone, int attemptCount = 10)
        {
            for (int i = 0; i < attemptCount; i++)
            {
                var pos = TryGetSingleRandomZonePos(zone);
                var worldPos = GetWorldVector(pos);
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
                var worldPos = GetWorldVector(corner);
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

        public Vector3 GetWorldVector(PointF pt)
        {
            int height = _world.GetTerrainHeight(pt);
            return pt.WithHeight(height + 1);
        }
    }
}