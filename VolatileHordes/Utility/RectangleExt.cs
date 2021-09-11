using System.Drawing;
using UnityEngine;

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
    }
}