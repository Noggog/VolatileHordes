using System;
using System.Drawing;
using UnityEngine;

namespace VolatileHordes.Utility
{
    public static class PointService
    {
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

        static double CalculateAngle(PointF start, PointF end)
        {
            return Math.Atan2(end.Y - start.Y, end.X - start.X)
                .Let(radian => (radian * (180 / Math.PI) + 360) % 360);
        }

        static PointF PointDistanceAway(PointF point, double angle, byte distance)
        {
            return new PointF(
                (float)(point.X + Math.Cos(angle) * distance),
                (float)(point.Y + Math.Sin(angle) * distance)
            );
        }
    }
}