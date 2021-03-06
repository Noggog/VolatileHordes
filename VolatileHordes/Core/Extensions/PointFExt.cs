using System;
using System.Drawing;
using UnityEngine;

namespace VolatileHordes.Utility
{
    public static class PointFExt
    {
        public static Vector3 ToZeroHeightVector(this PointF pt)
        {
            return new Vector3(pt.X, 0, pt.Y);
        }

        public static float AbsDistance(this PointF pt, PointF rhs)
        {
            return (float)Math.Abs(Math.Sqrt(Math.Pow((rhs.X - pt.X), 2) + Math.Pow((rhs.Y - pt.Y), 2)));
        }

        public static PointF Average(this PointF pt, PointF rhs)
        {
            return new PointF((pt.X + rhs.X) / 2, (pt.Y + rhs.Y) / 2);
        }

        public static Percent PercentAwayFrom(this PointF pt, PointF rhs, ushort maxRadius)
        {
            var distance = pt.AbsDistance(rhs);
            return Percent.FactoryPutInRange(distance / maxRadius);
        }
    }
}