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
    }
}