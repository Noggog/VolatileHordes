using System;
using System.Drawing;
using UnityEngine;

namespace VolatileHordes
{
    public static class VectorExt
    {
        public static PointF ToPoint(this Vector3 vect)
        {
            return new PointF(vect.x, vect.z);
        }
        
        public static Vector3 WithHeight(this PointF pt, float height)
        {
            return new Vector3(pt.X, height, pt.Y);
        }
    }
}