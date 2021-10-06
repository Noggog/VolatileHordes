using System;
using System.Drawing;
using VolatileHordes.Core;
using VolatileHordes.Core.Models;
using VolatileHordes.Randomization;

namespace VolatileHordes.Utility
{
    public class PointService
    {
        private RandomSource random { get; }

        public PointService(RandomSource random)
        {
            this.random = random;
        }

        /**
         * Returns a point that is [distance] distance away from [start], crossing through [end].
         */
        public PointF Leapfrog(PointF start, PointF end, byte distance)
        {
            if (start == end) Logger.Warning("Leapfrog: start == end:{0}", start);
            return (start.ToZeroHeightVector()
                    + (end.ToZeroHeightVector() - start.ToZeroHeightVector())
                    .normalized
                    * distance
                ).ToPoint();
        }

        public double AngleBetween(PointF start, PointF end)
        {
            return Math.Atan2(end.Y - start.Y, end.X - start.X)
                .Let(radian => (radian * (180 / Math.PI) + 360) % 360);
        }

        /// <summary>
        /// assumes 
        /// <paramref name="angle"/>
        /// as 
        /// <see cref="AngleType.Degree"/>
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public PointF PointDistanceAwayByAngle(PointF point, double angle, byte distance)
        {
            var _angle = new Angle(angle, AngleType.Degree);
            return new PointF(
                (float)(point.X + Math.Cos(_angle.Radian) * distance),
                (float)(point.Y + Math.Sin(_angle.Radian) * distance)
            );
        }

        public double RandomlyAdjustAngle(double angle, double amount)
        {
            return (angle + random.NextDouble() * amount * (1 - (random.NextBool() ? 2 : 0)));
        }

        /**
         * TODO: make the random area be a circle, not a box.
         */
        public PointF RandomlyAdjustPoint(PointF point, byte amount)
        {
            return new PointF(
                (float)(point.X + random.NextDouble() * amount * (1 - (random.NextBool() ? 2 : 0))),
                (float)(point.Y + random.NextDouble() * amount * (1 - (random.NextBool() ? 2 : 0)))
            );
        }
    }
}