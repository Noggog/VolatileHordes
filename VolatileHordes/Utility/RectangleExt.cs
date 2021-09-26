using System.Drawing;

namespace VolatileHordes
{
    public static class RectangleExt
    {
        public static PointF GetCenter(this RectangleF rect)
        {
            return new PointF(
                x: rect.Left + (rect.Width / 2),
                y: rect.Bottom + (rect.Height / 2));
        }
        
        public static RectangleF Absorb(this RectangleF rect, PointF pt)
        {
            return RectangleF.Union(
                rect,
                new RectangleF(pt, new SizeF(0.1f, 0.1f)));
        }
    }
}