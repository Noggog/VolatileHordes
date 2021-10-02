using System;
using System.Drawing;
using VolatileHordes.Randomization;

namespace VolatileHordes.Utility
{
    public class PointService
    {
        private RandomSource _random { get; }

        public PointService(RandomSource random)
        {
            _random = random;
        }

        /**
         * Returns a point that is [distance] distance away from [start], crossing through [end].
         */
        public static PointF Leapfrog(PointF start, PointF end, byte distance)
        {
            if (start == end) Logger.Warning("OverPointWithDistance: start == end:{0}", start);
            return (start.ToZeroHeightVector() + (end.ToZeroHeightVector() - start.ToZeroHeightVector())
                .Log("difference")
                .normalized
                .Log("normalized")
                * distance
                .Log("* distance")
                ).ToPoint();
        }

        public static double CalculateAngle(PointF start, PointF end)
        {
            return Math.Atan2(end.Y - start.Y, end.X - start.X)
                .Let(radian => (radian * (180 / Math.PI) + 360) % 360);
        }

        public static PointF PointDistanceAway(PointF point, double angle, byte distance)
        {
            return new PointF(
                (float)(point.X + Math.Cos(angle) * distance),
                (float)(point.Y + Math.Sin(angle) * distance)
            );
        }

        public double RandomlyAdjustAngle(double angle, double amount)
        {
            return angle + _random.NextDouble() * amount * (1 - (_random.NextBool() ? 2 : 0));
        }
    }
}