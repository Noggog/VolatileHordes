using System;
using VolatileHordes.AiPackages;
using Xunit;

namespace VolatileHordes.Tests.AiPackages
{
    public class EnumCompleteness
    {
        [Fact]
        public void IsComplete()
        {
            var mapper = new AiPackageMapper();
            
            foreach (AiPackageEnum e in Enum.GetValues(typeof(AiPackageEnum)))
            {
                Assert.NotNull(mapper.Get(e));
            }
        }
    }
}