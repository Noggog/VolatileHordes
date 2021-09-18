using System;
using Xunit;

namespace VolatileHordes.Tests.Utility
{
    public class TimeRangeTests
    {
        [Fact]
        public void Typical()
        {
            var one = TimeSpan.FromMinutes(1);
            var two = TimeSpan.FromMinutes(2);
            var range = new TimeRange(one, two);
            Assert.Equal(one, range.From);
            Assert.Equal(two, range.To);
        }
        
        [Fact]
        public void Flip()
        {
            var one = TimeSpan.FromMinutes(1);
            var two = TimeSpan.FromMinutes(2);
            var range = new TimeRange(two, one);
            Assert.Equal(one, range.From);
            Assert.Equal(two, range.To);
        }
        
        [Fact]
        public void Equal()
        {
            var one = TimeSpan.FromMinutes(1);
            var range = new TimeRange(one, one);
            Assert.Equal(one, range.From);
            Assert.Equal(one, range.To);
        }
        
        [Fact]
        public void PositiveDiff()
        {
            var one = TimeSpan.FromMinutes(1);
            var three = TimeSpan.FromMinutes(3);
            var range = new TimeRange(one, three);
            Assert.Equal(TimeSpan.FromMinutes(2), range.Diff);
        }
    }
}