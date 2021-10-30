using System;
using System.Drawing;
using VolatileHordes.Settings.User.Allocation;
using VolatileHordes.Utility;

namespace VolatileHordes.Allocation
{
    public class AllocationManager
    {
        private readonly AllocationBuckets _buckets;
        private readonly float _fillPerMinute;
        
        public AllocationManager(
            AllocationBuckets buckets,
            TimeManager time,
            AllocationSettings settings)
        {
            _buckets = buckets;
            var minutesPerDay = 72_000 / GameStats.GetInt(EnumGameStats.TimeOfDayIncPerSec) / 60;
            _fillPerMinute = settings.BucketFillPercentagePerDay / minutesPerDay;
            Logger.Info("Minutes per day {0}.  Fill per minute {1}", minutesPerDay, _fillPerMinute);
            time.Interval(TimeSpan.FromMinutes(1))
                .Subscribe(Dispense);
        }

        private void Dispense()
        {
            for (int x = 0; x <_buckets.Width; x++)
            {
                for (int y = 0; y < _buckets.Height; y++)
                {
                    var val = _buckets[x, y];
                    _buckets[x, y] = Percent.FactoryPutInRange(val.Value + _fillPerMinute);
                }
            }
        }

        public Point GetAllocationBucket(PointF point)
        {
            return _buckets.ConvertFromWorld(point);
        }

        public Rectangle GetBucketBounds(Point point)
        {
            return _buckets.GetBounds(point);
        }

        // public bool TryRequest(PointF pt, Percent percent, out int number)
        // {
        //     
        // }
    }
}