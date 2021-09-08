using System;
using System.Reactive.Subjects;
using JetBrains.dotMemoryUnit;
using Xunit;
using Xunit.Abstractions;

namespace VolatileHordes.Tests
{
    public class RxTests
    {
        private ITestOutputHelper _output;

        public RxTests(ITestOutputHelper output)
        {
            _output = output;
            DotMemoryUnitTestOutput.SetOutputMethod(_output.WriteLine);
        }
        
        [Fact]
        [DotMemoryUnit(CollectAllocations=true)]
        public void GarbageTests()
        {
            BehaviorSubject<DateTime> dt = new(DateTime.Now);
            for (int i = 0; i < 10; i++)
            {
                dt.OnNext(DateTime.Now);
            }

            var checkpoint1 = dotMemory.Check();
            for (int i = 0; i < 10; i++)
            {
                dt.OnNext(DateTime.Now);
            }
            dotMemory.Check(mem =>
            {
                Assert.Equal(0,
                    mem.GetDifference(checkpoint1)
                        .GetNewObjects().ObjectsCount);

            });
        }
        
        [Fact]
        [DotMemoryUnit(CollectAllocations=true)]
        public void GarbageTests2()
        {
            BehaviorSubject<DateTime> dt = new(DateTime.Now);
            for (int i = 0; i < 10; i++)
            {
                dt.OnNext(DateTime.Now);
            }

            var checkpoint1 = dotMemory.Check();
            for (int i = 0; i < 10000000; i++)
            {
                dt.OnNext(DateTime.Now);
            }
            dotMemory.Check(mem =>
            {
                Assert.Equal(0,
                    mem.GetDifference(checkpoint1)
                        .GetNewObjects().ObjectsCount);

            });
        }
    }
}