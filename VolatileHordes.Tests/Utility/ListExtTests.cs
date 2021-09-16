using System;
using System.Linq;
using Xunit;

namespace VolatileHordes.Tests.Utility
{
    public class ListExtTests
    {
        [Fact]
        public void EnumerateAllFromIndex()
        {
            var list = new int[] { 0, 1, 2, 3, 4 };
            var ret = list.EnumerateAllFromIndex(2)
                .ToArray();
            Assert.Equal(
                new int[] { 2, 3, 4, 0, 1},
                ret);
        }
    }
}