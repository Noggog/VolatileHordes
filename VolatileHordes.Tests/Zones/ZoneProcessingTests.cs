using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VolatileHordes.Zones;
using Xunit;

namespace VolatileHordes.Tests.Zones
{
    public class ZoneProcessingTests
    {
        [Fact]
        public void Corners()
        {
            var results = ZoneProcessing.Corners(
                    new RectangleF(1.0f, 5.0f, 2.0f, 3.0f))
                .ToArray();
            Assert.Equal(4, results.Length);
            // Assert.Equal(
            //     new PointF[]
            //     {
            //         new PointF(1.0f, 5.0f),
            //         new PointF(3.0f, 5.0f),
            //         new PointF(1.0f, 8.0f),
            //         new PointF(3.0f, 8.0f)
            //     },
            //     results,);
        }
    }
}