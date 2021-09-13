using System;
using UnityEngine;

namespace VolatileHordes
{
    public static class FloatExt
    {
        public static bool EqualsWithin(this float a, float b, float within = 0.000000001f)
        {
            if (Math.Abs(a - b) < within) return true;
            if (float.IsInfinity(a) && float.IsInfinity(b)) return true;
            if (float.IsNaN(a) && float.IsNaN(b)) return true;
            return false;
        }

        public static int Floor(this float f)
        {
            return Mathf.FloorToInt(f);
        }
    }
}