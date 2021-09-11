using System.Drawing;
using UnityEngine;
using VolatileHordes.Randomization;
using VolatileHordes.Zones;

namespace VolatileHordes.Spawning
{
    public class SpawningPositions
    {
        private readonly PlayerZoneManager _playerZoneManager;
        private readonly RandomSource _randomSource;
        public static readonly SpawningPositions Instance = new(
            PlayerZoneManager.Instance,
            RandomSource.Instance);

        public SpawningPositions(
            PlayerZoneManager playerZoneManager,
            RandomSource randomSource)
        {
            _playerZoneManager = playerZoneManager;
            _randomSource = randomSource;
        }

        public SpawnTarget? GetRandomTarget()
        {
            var zone = GetRandomZone();
            if (zone == null) return null;
            var pos = GetRandomZonePos(zone);
            if (pos == null) return null;
            return new SpawnTarget(pos.Value, zone.SpawnRectangle);
        }

        public PlayerZone? GetRandomZone()
        {
            return _playerZoneManager.GetRandom(_randomSource);
        }
        
        public PointF? GetRandomZonePos(PlayerZone zone, int attemptCount = 10)
        {
            var world = GameManager.Instance.World;
            for (int i = 0; i < attemptCount; i++)
            {
                var pos = TryGetSingleRandomZonePos(zone);
                
                if (world.CanMobsSpawnAtPos(GetWorldVector(pos)))
                {
                    return pos;
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
            var world = GameManager.Instance.World;
            Logger.Debug("Getting height at {0}", pt);
            int height = world.GetTerrainHeight((int)pt.X, (int)pt.Y);
            Logger.Debug("Height was {0}", height);
            return pt.WithHeight(height + 1);
        }
    }
}