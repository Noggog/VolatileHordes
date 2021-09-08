using System.Collections.Generic;
using UnityEngine;

namespace VolatileHordes
{
    public class ZoneManager<T>
        where T : PlayerZone
    {
        protected List<T> _zones = new();
        protected object _lock = new();
        protected int _pickCount = 0;

        public T? GetNext()
        {
            lock (_lock)
            {
                if (_zones.Count == 0)
                    return default;

                int pick = _pickCount % _zones.Count;
                _pickCount++;

                return _zones[pick];
            }
        }

        public T? GetRandom(RandomSource prng)
        {
            lock (_lock)
            {
                if (_zones.Count == 0)
                    return default;

                for (int i = 0; i < 4; i++)
                {
                    var idx = prng.Get(0, _zones.Count);
                    var zone = _zones[idx];
                    if (zone.valid)
                    {
                        return zone;
                    }
                }

                return default;
            }
        }

        public T? FindByPos2D(Vector3 pos)
        {
            lock (_lock)
            {
                foreach (var zone in _zones)
                {
                    var z = zone;
                    if (z.IsInside2D(pos))
                        return zone;
                }

                return default;
            }
        }

        public List<T> FindAllByPos2D(Vector3 pos)
        {
            List<T> res = new();
            lock (_lock)
            {
                foreach (var zone in _zones)
                {
                    if (zone.IsInside2D(pos))
                    {
                        res.Add(zone);
                    }
                }
            }
            return res;
        }
    }
}