using System;
using System.Drawing;
using NSubstitute;
using NUnit.Framework;
using VolatileHordes.Allocation;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Probability;
using VolatileHordes.Settings.World;
using VolatileHordes.Utility;

namespace VolatileHordes.Tests.Allocation
{
    public class ChunkTrackerTests
    {
        private static IWorld GetWorld()
        {
            var world = Substitute.For<IWorld>();
            world.Bounds.Returns(new Rectangle(-4096, -4096, 8192, 8192));
            return world;
        }

        private static float[,] GetBuckets()
        {
            var random = new RandomSource();
            var bucket = new float[82, 82];
            for (int x = 0; x < bucket.GetLength(0); x++)
            {
                for (int y = 0; y < bucket.GetLength(1); y++)
                {
                    bucket[x, y] = (float)random.Random.NextDouble();
                }
            }

            return bucket;
        }
        
        [Test]
        public void EmptySettingsAllFull()
        {
            var world = GetWorld();
            var tracker = new AllocationBuckets(
                Substitute.For<ILogger>(),
                world);
            tracker.Init(new AllocationWorldSettings());
            for (int x = 0; x < tracker.Width; x++)
            {
                for (int y = 0; y < tracker.Height; y++)
                {
                    Assert.AreEqual(
                        tracker[new Point(x, y)],
                        Percent.One);
                }
            }
        }

        [Test]
        public void LoadsSettings()
        {
            var world = GetWorld();
            var tracker = new AllocationBuckets(
                Substitute.For<ILogger>(),
                world);
            var buckets = GetBuckets();

            tracker.Init(new()
            {
                Buckets = buckets
            });
            for (int x = 0; x < tracker.Width; x++)
            {
                for (int y = 0; y < tracker.Height; y++)
                {
                    Assert.True(
                        buckets[x, y].EqualsWithin(
                            tracker[new Point(x, y)]));
                }
            }
        }

        [Test]
        public void GetFromWorldPoint()
        {
            var world = GetWorld();
            var tracker = new AllocationBuckets(
                Substitute.For<ILogger>(),
                world);
            var buckets = GetBuckets();
            
            tracker.Init(new()
            {
                Buckets = buckets
            });
            
            Assert.True(buckets[0, 0].EqualsWithin(tracker[new PointF(-4096, -4096)]));
            Assert.True(buckets[81, 81].EqualsWithin(tracker[new PointF(4096, 4096)]));
            Assert.True(buckets[0, 81].EqualsWithin(tracker[new PointF(-4096, 4096)]));
            Assert.True(buckets[81, 0].EqualsWithin(tracker[new PointF(4096, -4096)]));
            Assert.True(buckets[40, 40].EqualsWithin(tracker[new PointF(0, 0)]));
        }

        [Test]
        public void GetFromWorldPointOutsideThrows()
        {
            var world = GetWorld();
            var tracker = new AllocationBuckets(
                Substitute.For<ILogger>(),
                world);
            tracker.Init(new());
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                var v = tracker[new PointF(0, 5096)];
            });
        }

        [Test]
        public void Consume()
        {
            var world = GetWorld();
            var tracker = new AllocationBuckets(
                Substitute.For<ILogger>(),
                world);
            var buckets = GetBuckets();
            tracker.Init(new()
            {
                Buckets = buckets
            });
            var pt = new PointF(100, 100);
            var val = tracker[pt];
            var half = val / 2;
            tracker.Consume(pt, new Percent(half));
            Assert.True(tracker[pt].Value.EqualsWithin(half));
        }
    }
}