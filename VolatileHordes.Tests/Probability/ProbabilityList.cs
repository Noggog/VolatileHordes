using NUnit.Framework;
using VolatileHordes.Core.Models;
using VolatileHordes.Probability;

namespace VolatileHordes.Tests.Probability
{
    public class ProbabilityList
    {
        [Test]
        public void Max()
        {
            var list = new ProbabilityList<int>();
            list.Add(1, new UDouble(0.25d));
            list.Add(2, new UDouble(0.25d));
            list.Add(3, new UDouble(1d));

            Assert.AreEqual(3, list.GetAt(new UDouble(list.TotalWeight.Value)));
        }
        
        [Test]
        public void Zero()
        {
            var list = new ProbabilityList<int>();
            list.Add(1, new UDouble(0.25d));
            list.Add(2, new UDouble(0.25d));
            list.Add(3, new UDouble(1d));

            Assert.AreEqual(1, list.GetAt(new UDouble(0d)));
        }
    }
}