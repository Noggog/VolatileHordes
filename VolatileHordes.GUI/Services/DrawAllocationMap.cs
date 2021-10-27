using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using VolatileHordes.GUI.Extensions;

namespace VolatileHordes.GUI.Services
{
    public record DrawAllocationMapInput(
        ushort Size, 
        float[,]? Buckets);
    
    public class DrawAllocationMap
    {
        class DrawPass : IDisposable
        {
            private readonly DrawAllocationMapInput _input;
            private readonly Bitmap _bitmap;
            private readonly Graphics _gr;
            private readonly double _offsetX;
            private readonly int _offsetXCeiling;
            private readonly double _offsetY;
            private readonly int _offsetYCeiling;

            public DrawPass(DrawAllocationMapInput input)
            {
                _bitmap = new Bitmap(input.Size, input.Size);
                _gr = Graphics.FromImage(_bitmap);
                _input = input;
                if (input.Buckets != null)
                {
                    _offsetX = 1.0d * input.Size / input.Buckets.GetLength(0);
                    _offsetXCeiling = (int)Math.Ceiling(_offsetX);
                    _offsetY = 1.0d * input.Size / input.Buckets.GetLength(1);
                    _offsetYCeiling = (int)Math.Ceiling(_offsetY);
                }
            }

            private int GetX(int xSlot)
            {
                return (int)(xSlot * _offsetX);
            }

            private int GetY(int ySlot)
            {
                return (int)(ySlot * _offsetY);
            }
            
            public BitmapImage Draw()
            {
                _gr.Clear(Color.Black);

                if (_input.Buckets != null)
                {
                    for (int x = 0; x < _input.Buckets.GetLength(0); x++)
                    {
                        for (int y = 0; y < _input.Buckets.GetLength(1); y++)
                        {
                            var b = byte.MaxValue - (byte)(_input.Buckets[x, y] * 255);
                            _gr.FillRectangle(
                                new SolidBrush(Color.FromArgb(b, b, b)),
                                GetX(x), GetY(y),
                                _offsetXCeiling, _offsetYCeiling);
                        }
                    }
                    
                    for (int x = 0; x < _input.Buckets.GetLength(0); x++)
                    {
                        _gr.DrawLine(
                            new Pen(Color.FromArgb(15, 15, 15)),
                            new Point(GetX(x), 0),
                            new Point(GetX(x), _input.Size));
                    }
                    
                    for (int y = 0; y < _input.Buckets.GetLength(1); y++)
                    {
                        _gr.DrawLine(
                            new Pen(Color.FromArgb(15, 15, 15)),
                            new Point(0, GetY(y)),
                            new Point(_input.Size, GetY(y)));
                    }
                }
                
                _bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                return _bitmap.ToBitmapImage();
            }

            public void Dispose()
            {
                _gr.Dispose();
                _bitmap.Dispose();
            }
        }

        public BitmapImage Draw(DrawAllocationMapInput input)
        {
            using var drawer = new DrawPass(input);
            return drawer.Draw();
        }
    }
}