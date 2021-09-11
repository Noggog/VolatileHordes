using System.Collections;
using System.Collections.Generic;
using VolatileHordes.Randomization;

namespace VolatileHordes.Zones
{
    public class ZoneManager<T> : IEnumerable<T>
        where T : PlayerZone
    {
        protected readonly List<T> _zones = new();

        public T? GetRandom(RandomSource prng)
        {
            if (_zones.Count == 0)
                return default;

            for (int i = 0; i < 4; i++)
            {
                var idx = prng.Get(0, _zones.Count);
                var zone = _zones[idx];
                if (zone.Valid)
                {
                    return zone;
                }
            }

            return default;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _zones.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}