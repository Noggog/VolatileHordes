using System.Drawing;
using VolatileHordes.Utility;
using Xunit;

namespace VolatileHordes.Tests.Utility
{
    public class PointFExtTests
    {
        [Fact]
        public void Typical()
        {
            Assert.True(
                new PointF(1, 1).AbsDistance(new PointF(1, 0)).EqualsWithin(1f));
            Assert.True(
                new PointF(1, 0).AbsDistance(new PointF(1, 1)).EqualsWithin(1f));
        }
    }
}