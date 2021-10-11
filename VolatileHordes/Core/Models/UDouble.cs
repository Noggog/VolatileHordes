using System;

namespace VolatileHordes.Core.Models
{
    public struct UDouble
    {
        public double Value { get; }

        public UDouble(double val)
        {
            if (val < 0)
            {
                throw new ArgumentException("Value cannot be null");
            }

            Value = val;
        }
    }
}