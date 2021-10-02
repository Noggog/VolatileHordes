using System;

namespace VolatileHordes.Utility
{
    public static class DoubleExt
    {
        public static bool EqualsWithin(this double a, double b, double within = 0.000000001f)
        {
            if (Math.Abs(a - b) < within) return true;
            if (double.IsInfinity(a) && double.IsInfinity(b)) return true;
            if (double.IsNaN(a) && double.IsNaN(b)) return true;
            return false;
        }
    }
}