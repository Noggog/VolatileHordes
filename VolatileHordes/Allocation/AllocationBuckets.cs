using System;
using System.Drawing;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Settings.World;
using VolatileHordes.Utility;

namespace VolatileHordes.Allocation
{
    public class AllocationBuckets
    {
        private readonly ILogger _logger;
        private readonly IWorld _world;
        public const ushort ChunkSize = 100;
        private Percent[,] _buckets = null!;
        private int _offsetX;
        private int _offsetY;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Percent this[Point pt]
        {
            get => _buckets[pt.X, pt.Y];
            set => _buckets[pt.X, pt.Y] = value;
        }

        public Percent this[int x, int y]
        {
            get => _buckets[x, y];
            set => _buckets[x, y] = value;
        }

        public Percent this[PointF pt]
        {
            get => this[ConvertFromWorld(pt)];
            set => this[ConvertFromWorld(pt)] = value;
        }

        public Percent this[float x, float y]
        {
            get => this[ConvertFromWorld(new PointF(x, y))];
            set => this[ConvertFromWorld(new PointF(x, y))] = value;
        }

        public AllocationBuckets(
            ILogger logger,
            IWorld world)
        {
            _logger = logger;
            _world = world;
        }

        public void Init(AllocationWorldSettings settings)
        {
            var bounds = _world.Bounds;
            _logger.Info("World bounds: {0}", bounds);

            Width = (int)Math.Ceiling(1d * bounds.Width / ChunkSize);
            Height = (int)Math.Ceiling(1d * bounds.Height / ChunkSize);
            _offsetX = bounds.Width / 2;
            _offsetY = bounds.Height / 2;
            
            _logger.Info("Bucket Size: {0}, {1}.  Offset: {2}, {3}", Width, Height, _offsetX, _offsetY);
            _buckets = new Percent[Width, Height];
            
            if (settings.Buckets == null)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        _buckets[x, y] = Percent.One;
                    }
                }
            }
            else
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (x < settings.Buckets.GetLength(0)
                            && y < settings.Buckets.GetLength(1))
                        {
                            _buckets[x, y] = Percent.FactoryPutInRange(settings.Buckets[x, y]);
                        }
                        else
                        {
                            _buckets[x, y] = Percent.One;
                        }
                    }
                }
            }
        }

        public void Save(AllocationWorldSettings settings)
        {
            settings.Buckets = new float[_buckets.GetLength(0), _buckets.GetLength(1)];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    settings.Buckets[x, y] = _buckets[x, y];
                }
            }
        }

        private Point ConvertFromWorld(PointF pt)
        {
            var x = (int)pt.X + _offsetX;
            var y = (int)pt.Y + _offsetY;
            return new Point(x / ChunkSize, y / ChunkSize);
        }

        public void Consume(PointF pt, Percent p)
        {
            var pt2 = ConvertFromWorld(pt);
            var val = _buckets[pt2.X, pt2.Y];
            _buckets[pt2.X, pt2.Y] = Percent.FactoryPutInRange(val - p);
        }
    }
}