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
            var diff = pt.ToZeroHeightVector() - rhs.ToZeroHeightVector();
            return Mathf.Abs(diff.magnitude);
        }

        /**
         * Returns a point that is [distance] distance away from [a], crossing through [b].
         */
        public static PointF OverPointWithDistance(this PointF a, PointF b, byte distance)
        {
            return ((a.ToZeroHeightVector() - b.ToZeroHeightVector()).normalized * distance).ToPoint();
        }
    }
}