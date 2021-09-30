using System.Drawing;
using VolatileHordes.Utility;
using Xunit;

namespace VolatileHordes.Tests.Utility
{
    public class PointFExtTests
    {
        [Fact]
        public void AbsDistance()
        {
            Assert.True(
                new PointF(1, 1).AbsDistance(new PointF(1, 0)).EqualsWithin(1f));
            Assert.True(
                new PointF(1, 0).AbsDistance(new PointF(1, 1)).EqualsWithin(1f));
        }

        #region PercentAwayFrom

        [Fact]
        public void PercentAwayFromTypical()
        {
            Assert.True(
                new PointF(0, 0).PercentAwayFrom(new PointF(5, 0), 10).Value.EqualsWithin(0.5f));
        }
        
        [Fact]
        public void PercentAwayFromSame()
        {
            Assert.True(
                new PointF(0, 0).PercentAwayFrom(new PointF(0, 0), 10).Value.EqualsWithin(0f));
        }
        
        [Fact]
        public void PercentAwayFromOver()
        {
            Assert.True(
                new PointF(0, 0).PercentAwayFrom(new PointF(20, 0), 10).Value.EqualsWithin(1f));
        }

        #endregion
        
        [Fact]
        public void IsTargetAwayFromAway()
        {
            Assert.True(
                new PointF(0, 0).IsTargetAwayFrom(new PointF(0, 1), new PointF(0, -1)));
        }
        
        [Fact]
        public void IsTargetAwayFromAway2()
        {
            Assert.True(
                new PointF(0, 0).IsTargetAwayFrom(new PointF(1, 1), new PointF(0, -1)));
        }
        
        [Fact]
        public void IsTargetAwayFromNotAway()
        {
            Assert.False(
                new PointF(0, 0).IsTargetAwayFrom(new PointF(0, 1), new PointF(0, 2)));
        }
        
        [Fact]
        public void IsTargetAwayFromSame()
        {
            Assert.False(
                new PointF(0, 0).IsTargetAwayFrom(new PointF(0, 1), new PointF(0, 1)));
        }
    }
}