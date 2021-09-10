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
        
        public Vector3? GetRandomZonePos(PlayerZone zone, int attemptCount = 10)
        {
            var world = GameManager.Instance.World;
            for (int i = 0; i < 10; i++)
            {
                var pos = TryGetSingleRandomZonePos(zone);
                if (world.CanMobsSpawnAtPos(pos))
                {
                    return pos;
                }
            }

            return Vector3.zero;
        }
        
        public Vector3 TryGetSingleRandomZonePos(PlayerZone zone)
        {
            var world = GameManager.Instance.World;
            Vector3 pos = new Vector3();
            Vector3 spawnPos = new Vector3();
            pos.x = _randomSource.Get(zone.minsSpawnBlock.x, zone.maxsSpawnBlock.x);
            pos.z = _randomSource.Get(zone.minsSpawnBlock.z, zone.maxsSpawnBlock.z);

            int height = world.GetTerrainHeight((int)pos.x, (int)pos.z);

            spawnPos.x = pos.x;
            spawnPos.y = height + 1.0f;
            spawnPos.z = pos.z;
            return spawnPos;
        }
    }
}