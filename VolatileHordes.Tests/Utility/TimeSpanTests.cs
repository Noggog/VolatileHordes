using System;
using Xunit;

namespace VolatileHordes.Tests.Utility
{
    public class TimeSpanTests
    {
        [Fact]
        public void AddTicksTypical()
        {
            var ticks = 300;
            var span = TimeSpan.FromMinutes(1);
            var added = span.AddTicks(300);
            Assert.Equal(
                TimeSpan.FromTicks(span.Ticks + ticks),
                added);
        }
    }
}