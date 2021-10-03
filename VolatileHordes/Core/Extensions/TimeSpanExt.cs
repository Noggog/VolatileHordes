using System;

namespace VolatileHordes
{
    public static class TimeSpanExt
    {
        public static TimeSpan AddTicks(this TimeSpan span, long ticks)
        {
            return span.Add(TimeSpan.FromTicks(ticks));
        }
    }
}