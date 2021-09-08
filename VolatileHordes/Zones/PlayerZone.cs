using System.Drawing;
using UnityEngine;
using VolatileHordes.Randomization;

namespace VolatileHordes.Zones
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

        public int GetIndex() => index;

        // Zone AABB min
        public Vector3 GetMins() => mins;

        // Zone AABB min
        public Vector3 GetMaxs() => maxs;

        // Returns the center of the center.
        public Vector3 GetCenter() => center;

        public RectangleF GetSpawnRectangle => new(
            x: minsSpawnBlock.x,
            y: minsSpawnBlock.z,
            width: maxsSpawnBlock.x - minsSpawnBlock.x,
            height: maxsSpawnBlock.z - minsSpawnBlock.z);

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
        
        public Vector3? GetRandomZonePos(RandomSource randomSource, int attemptCount = 10)
        {
            var world = GameManager.Instance.World;
            for (int i = 0; i < 10; i++)
            {
                var pos = TryGetSingleRandomZonePos(randomSource);
                if (world.CanMobsSpawnAtPos(pos))
                {
                    return pos;
                }
            }

            return Vector3.zero;
        }
        
        public Vector3 TryGetSingleRandomZonePos(RandomSource randomSource)
        {
            var world = GameManager.Instance.World;
            Vector3 pos = new Vector3();
            Vector3 spawnPos = new Vector3();
            pos.x = randomSource.Get(minsSpawnBlock.x, maxsSpawnBlock.x);
            pos.z = randomSource.Get(minsSpawnBlock.z, maxsSpawnBlock.z);

            int height = world.GetTerrainHeight((int)pos.x, (int)pos.z);

            spawnPos.x = pos.x;
            spawnPos.y = height + 1.0f;
            spawnPos.z = pos.z;
            return spawnPos;
        }
    }
}