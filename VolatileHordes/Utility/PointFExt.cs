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

        public static PointF Average(this PointF pt, PointF rhs)
        {
            return new PointF((pt.X + rhs.X) / 2, (pt.Y + rhs.Y) / 2);
        }

        public static Percent PercentAwayFrom(this PointF pt, PointF rhs, ushort maxRadius)
        {
            var distance = pt.AbsDistance(rhs);
            return Percent.FactoryPutInRange(distance / maxRadius);
        }

        public static bool IsTargetAwayFrom(this PointF curLoc, PointF target, PointF from)
        {
            var locVector = curLoc.ToZeroHeightVector();
            var targetDiff = target.ToZeroHeightVector() - locVector;
            var fromDiff = from.ToZeroHeightVector() - locVector;
            return Vector3.Dot(targetDiff, fromDiff) < 0;
        }
    }
}