using System.Drawing;
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
            var pos = GetRandomZoneVector(zone);
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
        
        public Vector3? GetRandomZoneVector(PlayerZone zone, int attemptCount = 10)
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
        
        public PointF TryGetSingleRandomZonePos(PlayerZone zone)
        {
            return new PointF(
                _randomSource.Get(zone.MinsSpawnBlock.X, zone.MaxsSpawnBlock.X),
                _randomSource.Get(zone.MinsSpawnBlock.Y, zone.MaxsSpawnBlock.Y));
        }

        public Vector3 GetWorldVector(PointF pt)
        {
            Logger.Debug("Getting height at {0}", pt);
            int height = _world.GetTerrainHeight(pt);
            Logger.Debug("Height was {0}", height);
            return pt.WithHeight(height + 1);
        }
    }
}