using System.Collections.Generic;
using System.Drawing;

namespace VolatileHordes.Zones
{
    public static class ZoneProcessing
    {
        public static IEnumerable<RectangleF> FindOverlappingRects(RectangleF rect, IEnumerable<RectangleF> rects)
        {
            foreach (var rhs in rects)
            {
                if (rect.IntersectsWith(rhs))
                {
                    yield return rhs;
                }
            }
        }

        public static PointF? PickCornerFromCluster(IReadOnlyList<RectangleF> rects)
        {
            for (int i = 0; i < rects.Count; i++)
            {
                var rectA = rects[i];
                foreach (var corner in Corners(rectA))
                {
                    if (!AnyContains(rects, corner, i))
                    {
                        return corner;
                    }
                }
            }

            return null;
        }

        public static IEnumerable<PointF> Corners(RectangleF rect)
        {
            yield return new PointF(rect.X, rect.Y);
            yield return new PointF(rect.X + rect.Width, rect.Y);
            yield return new PointF(rect.X, rect.Y + rect.Height);
            yield return new PointF(rect.X + rect.Width, rect.Y + rect.Height);
        }

        public static bool AnyContains(IReadOnlyList<RectangleF> rects, PointF pt, int ignoreIndex)
        {
            for (int i = 0; i < rects.Count; i++)
            {
                if (i == ignoreIndex) continue;
                var rect = rects[i];
                if (rect.Contains(pt)) return true;
            }

            return false;
        }
    }
}