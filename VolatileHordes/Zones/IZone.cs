using UnityEngine;
using VolatileHordes.Randomization;

namespace VolatileHordes.Zones
{
    public interface IZone
    {
        int GetIndex();

        bool IsInside2D(Vector3 pos);

        // Zone AABB min
        Vector3 GetMins();

        // Zone AABB min
        Vector3 GetMaxs();

        // Returns the center of the center.
        Vector3 GetCenter();

        // Returns a random position within the zone.
        Vector3 GetRandomPos(RandomSource prng);
    }
}