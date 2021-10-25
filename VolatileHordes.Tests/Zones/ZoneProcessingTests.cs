using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NSubstitute;
using VolatileHordes.Core.Services;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Players;
using Xunit;

namespace VolatileHordes.Tests.Zones
{
    public class ZoneProcessingTests
    {
        public class Corners
        {
            [Fact]
            public void Typical()
            {
                var results = ZoneProcessing.Corners(
                        new RectangleF(1.0f, 5.0f, 2.0f, 3.0f))
                    .ToArray();
                Assert.Equal(4, results.Length);
                Assert.Equal(
                    new PointF[]
                    {
                        new PointF(1.0f, 5.0f),
                        new PointF(3.0f, 5.0f),
                        new PointF(1.0f, 8.0f),
                        new PointF(3.0f, 8.0f)
                    },
                    results);
            }
        }

        public class EdgeCornersFromCluster
        {
            [Fact]
            public void Empty()
            {
                Assert.Empty(ZoneProcessing.EdgeCornersFromCluster(new List<RectangleF>()));
            }
            
            [Fact]
            public void Single()
            {
                var rect = new RectangleF(1f, 1f, 1f, 1f);
                var result = ZoneProcessing.EdgeCornersFromCluster(new List<RectangleF>()
                {
                    rect
                }).ToArray();
                Assert.Equal(4, result.Length);
                Assert.Equal(
                    result.ToHashSet(),
                    rect.Corners().ToHashSet());
            }
            
            [Fact]
            public void NoOverlap()
            {
                var rect = new RectangleF(1f, 1f, 1f, 1f);
                var rect2 = new RectangleF(10f, 10f, 1f, 1f);
                var result = ZoneProcessing.EdgeCornersFromCluster(new List<RectangleF>()
                {
                    rect,
                    rect2
                }).ToArray();
                Assert.Equal(8, result.Length);
                Assert.Equal(
                    result.ToHashSet(),
                    rect.Corners().Concat(rect2.Corners()).ToHashSet());
            }
            
            public static IEnumerable<object[]> SimpleOverlapData = new List<object[]>()
            {
                new object[]{ 
                    new RectangleF[]
                    {
                        new RectangleF(1f, 1f, 3f, 3f),
                        new RectangleF(2f, 2f, 3f, 3f)
                    },
                    new PointF[] { 
                        new PointF(1f, 1f),
                        new PointF(1f, 4f),
                        new PointF(4f, 1f),
                        new PointF(2f, 5f),
                        new PointF(5f, 2f),
                        new PointF(5f, 5f),
                    }
                },
                new object[]{ 
                    new RectangleF[]
                    {
                        new RectangleF(1f, 1f, 3f, 3f),
                        new RectangleF(-1f, 2f, 3f, 3f)
                    },
                    new PointF[] { 
                        new PointF(1f, 1f),
                        new PointF(4f, 4f),
                        new PointF(4f, 1f),
                        new PointF(-1f, 2f),
                        new PointF(-1f, 5f),
                        new PointF(2f, 5f),
                    }
                },
                new object[]{ 
                    new RectangleF[]
                    {
                        new RectangleF(1f, 1f, 3f, 3f),
                        new RectangleF(-1f, -1f, 3f, 3f)
                    },
                    new PointF[] { 
                        new PointF(1f, 4f),
                        new PointF(4f, 4f),
                        new PointF(4f, 1f),
                        new PointF(-1f, -1f),
                        new PointF(-1f, 2f),
                        new PointF(2f, -1f),
                    }
                },
                new object[]{ 
                    new RectangleF[]
                    {
                        new RectangleF(1f, 1f, 3f, 3f),
                        new RectangleF(2f, -1f, 3f, 3f)
                    },
                    new PointF[] { 
                        new PointF(1f, 4f),
                        new PointF(4f, 4f),
                        new PointF(1f, 1f),
                        new PointF(2f, -1f),
                        new PointF(5f, 2f),
                        new PointF(5f, -1f),
                    }
                },
            };
            
            public static IEnumerable<object[]> ExactOverlapData = new List<object[]>()
            {
                new object[]{ 
                    new RectangleF[]
                    {
                        new RectangleF(1f, 1f, 3f, 3f),
                        new RectangleF(1f, 1f, 3f, 3f)
                    },
                    new PointF[] { 
                        new PointF(1f, 1f),
                        new PointF(1f, 4f),
                        new PointF(4f, 1f),
                        new PointF(4f, 4f),
                    }
                },
            };
            
            public static IEnumerable<object[]> TwoOverlapOneIsolated = new List<object[]>()
            {
                new object[]{ 
                    new RectangleF[]
                    {
                        new RectangleF(1f, 1f, 3f, 3f),
                        new RectangleF(2f, -1f, 3f, 3f),
                        new RectangleF(10f, 10f, 3f, 3f),
                    },
                    new PointF[] { 
                        new PointF(1f, 1f),
                        new PointF(1f, 4f),
                        new PointF(4f, 4f),
                        new PointF(5f, 2f),
                        new PointF(2f, -1f),
                        new PointF(5f, -1f),
                        new PointF(10f, 10f),
                        new PointF(10f, 13f),
                        new PointF(13f, 13f),
                        new PointF(13f, 10f),
                    }
                },
            };
            
            public static IEnumerable<object[]> ThreeOverlap = new List<object[]>()
            {
                new object[]{ 
                    new RectangleF[]
                    {
                        new RectangleF(1f, 1f, 3f, 3f),
                        new RectangleF(2f, -1f, 3f, 3f),
                        new RectangleF(0f, 0f, 3f, 3f),
                    },
                    new PointF[] { 
                        new PointF(1f, 4f),
                        new PointF(4f, 4f),
                        new PointF(5f, 2f),
                        new PointF(2f, -1f),
                        new PointF(5f, -1f),
                        new PointF(0f, 0f),
                        new PointF(0f, 3f),
                    }
                },
            };
            
            [Theory]
            [MemberData(nameof(SimpleOverlapData))]
            [MemberData(nameof(ExactOverlapData))]
            [MemberData(nameof(TwoOverlapOneIsolated))]
            [MemberData(nameof(ThreeOverlap))]
            public void Overlaps(RectangleF[] rectangles, PointF[] expected)
            {
                var result = ZoneProcessing.EdgeCornersFromCluster(rectangles).ToArray();
                Assert.Equal(expected.ToHashSet(), result.ToHashSet());
                Assert.Equal(expected.Length, result.Length);
            }
        }

        //public class GetConnectedSpawnRects
        //{
        //    public record PlayerZone(RectangleF SpawnRectangle, IPlayer Player) : IPlayerZone;

        //    private IPlayer GetPlayer(int id)
        //    {
        //        var ret = Substitute.For<IPlayer>();
        //        ret.EntityId.Returns(id);
        //        return ret;
        //    }
            
        //    [Fact]
        //    public void Empty()
        //    {
        //        var rect = new RectangleF(1, 1, 3, 3);
        //        var conn = ZoneProcessing.GetConnectedSpawnRects(new PlayerZone(rect, GetPlayer(-3)),
        //            new IPlayerZone[0]);
        //        Assert.Equal(
        //            conn.ToHashSet(),
        //            new []{ rect }.ToHashSet());
        //    }
            
        //    [Fact]
        //    public void Unconnected()
        //    {
        //        var rect = new RectangleF(1, 1, 3, 3);
        //        var conn = ZoneProcessing.GetConnectedSpawnRects(new PlayerZone(rect, GetPlayer(1)),
        //            new IPlayerZone[]
        //            {
        //                new PlayerZone(
        //                    new RectangleF(15, 15, 3, 3), Substitute.For<IPlayer>())
        //            });
        //        Assert.Equal(
        //            conn.ToHashSet(),
        //            new []{ rect }.ToHashSet());
        //    }
            
        //    [Fact]
        //    public void Overlap()
        //    {
        //        var rect = new RectangleF(1, 1, 3, 3);
        //        var rect2 = new RectangleF(2, 2, 3, 3);
        //        var conn = ZoneProcessing.GetConnectedSpawnRects(new PlayerZone(rect, GetPlayer(1)),
        //            new IPlayerZone[]
        //            {
        //                new PlayerZone(rect2, GetPlayer(2))
        //            });
        //        Assert.Equal(
        //            conn.ToHashSet(),
        //            new []{ rect, rect2 }.ToHashSet());
        //    }
            
        //    [Fact]
        //    public void Chained()
        //    {
        //        var rect = new RectangleF(1, 1, 3, 3);
        //        var rect2 = new RectangleF(2, 2, 3, 3);
        //        var rect3 = new RectangleF(4, 4, 3, 3);
        //        var conn = ZoneProcessing.GetConnectedSpawnRects(new PlayerZone(rect, GetPlayer(1)),
        //            new IPlayerZone[]
        //            {
        //                new PlayerZone(rect2, GetPlayer(2)),
        //                new PlayerZone(rect3, GetPlayer(3))
        //            });
        //        Assert.Equal(
        //            conn.ToHashSet(),
        //            new []{ rect, rect2, rect3 }.ToHashSet());
        //    }
        //}
    }
}