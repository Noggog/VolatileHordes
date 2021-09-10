using System.Drawing;
using UnityEngine;

namespace VolatileHordes
{
    public static class RectangleExt
    {
        public static Vector2 GetCenter(this RectangleF rect)
        {
            return new Vector2(
                x: rect.Left + (rect.Width / 2),
                y: rect.Bottom + (rect.Height / 2));
        }
    }
}