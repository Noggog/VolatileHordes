using System.Collections.Generic;
using System.Drawing;
using VolatileHordes.Players;

namespace VolatileHordes.Core.Services
{
    public static class ZoneProcessing
    {
        public static IEnumerable<PointF> EdgeCornersFromCluster(IReadOnlyList<RectangleF> rects)
        {
            bool any = false;
            for (int i = 0; i < rects.Count; i++)
            {
                var rectA = rects[i];
                foreach (var corner in Corners(rectA))
                {
                    if (!AnyContains(rects, corner, i))
                    {
                        any = true;
                        yield return corner;
                    }
                }
            }

            if (!any && rects.Count > 0)
            {
                foreach (var corner in rects[0].Corners())
                {
                    yield return corner;
                }
            }
        }

        public static IEnumerable<PointF> Corners(this RectangleF rect)
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
                rect.Inflate(0.01f, 0.01f);
                if (rect.Contains(pt)) return true;
            }

            return false;
        }
        
        public static IEnumerable<RectangleF> GetConnectedSpawnRects(IPlayerZone zone, IReadOnlyCollection<IPlayerZone> zones)
        {
            return ZoneProcessing.GetConnectedSpawnRects(zone, zones, new HashSet<int>());
        }
        
        private static IEnumerable<RectangleF> GetConnectedSpawnRects(IPlayerZone zone, IReadOnlyCollection<IPlayerZone> zones, HashSet<int> passedPlayers)
        {
            yield return zone.SpawnRectangle;
            passedPlayers.Add(zone.Player.EntityId);

            foreach (var rhsZone in zones)
            {
                if (passedPlayers.Contains(rhsZone.Player.EntityId)) continue;
                if (!rhsZone.SpawnRectangle.IntersectsWith(zone.SpawnRectangle)) continue;
                
                foreach (var rhsRect in GetConnectedSpawnRects(rhsZone, zones, passedPlayers))
                {
                    yield return rhsRect;
                }
            }
        }
    }
}