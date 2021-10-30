using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using Splat;
using VolatileHordes.GUI.Extensions;

namespace VolatileHordes.GUI.Services
{
    public record PlayerDrawInput(RectangleF SpawnRectangle, RectangleF Rectangle, float Rotation);
    public record ZombieDrawInput(PointF Position, PointF Target, PointF GroupTarget, float Rotation);
    public record PlayerZoneDrawInput(
        RectangleF DrawArea,
        PlayerDrawInput Player,
        ushort Size, 
        ZombieDrawInput[] Zombies,
        ushort NoiseRadius,
        bool DrawTargetLines,
        bool DrawGroupTargetLines,
        Rectangle WorldRect,
        ushort BucketSize);
    
    public class DrawPlayerZone
    {
        private const byte PlayerConeSize = 50;
        private const byte PlayerConeRadius = 70;
        private const byte PlayerCircleRadius = 4;
        private const byte ZombieCircleRadius = 4;
        private const byte ZombieConeSize = 20;
        private const byte ZombieConeRadius = 70;
        private const byte ZombieTargetRadius = 2;

        class DrawPass : IDisposable
        {
            private readonly PlayerZoneDrawInput _input;
            private readonly Bitmap _bitmap;
            private readonly Graphics _gr;
            private readonly float _offsetX;
            private readonly float _offsetY;
            private readonly float _scaleX;
            private readonly float _scaleY;
            private readonly RectangleF _pictureBounds;
            
            public DrawPass(PlayerZoneDrawInput input)
            {
                _input = input;
                _bitmap = new Bitmap(input.Size, input.Size);
                _gr = Graphics.FromImage(_bitmap);
                _offsetX = input.DrawArea.Left;
                _offsetY = input.DrawArea.Top;
                _scaleX = input.Size / input.DrawArea.Width;
                _scaleY = input.Size / input.DrawArea.Height;
            }

            public void Dispose()
            {
                _gr.Dispose();
                _bitmap.Dispose();
            }

            public BitmapImage Draw()
            {
                _gr.Clear(Color.Black);
                DrawChunks();
                DrawPlayer();
                DrawZombies();
                _bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                return _bitmap.ToBitmapImage();
            }

            private void DrawChunks()
            {
                var pos = _input.WorldRect.Left;
                while (pos < _input.DrawArea.Right)
                {
                    _gr.DrawLine(
                        new Pen(Color.FromArgb(15, 15, 15)),
                        Offset(new PointF(pos, 0)),
                        Offset(new PointF(pos, 100000)));
                    pos += _input.BucketSize;
                }
                pos = _input.WorldRect.Top;
                while (pos < _input.DrawArea.Bottom)
                {
                    _gr.DrawLine(
                        new Pen(Color.FromArgb(15, 15, 15)),
                        Offset(new PointF(0, pos)),
                        Offset(new PointF(100000, pos)));
                    pos += _input.BucketSize;
                }
            }

            private void DrawPlayer()
            {
                if (_input.Player.SpawnRectangle.IsEmpty) return;
                var center = _input.Player.SpawnRectangle.Center();
                DrawCone(center, _input.Player.Rotation, PlayerConeSize, PlayerConeRadius,
                    Color.FromArgb(50, 255, 255, 255));
                DrawRectangle(_input.Player.SpawnRectangle, Color.FromArgb(17, 33, 41));
                DrawRectangle(_input.Player.Rectangle, Color.FromArgb(17, 33, 41));
                DrawCircle(center, _input.NoiseRadius, Color.FromArgb(15, 15, 15));
                FillCircleAbsoluteSize(center, PlayerCircleRadius, Color.Teal);
            }

            private void DrawRectangle(
                RectangleF rect,
                Color color)
            {
                _gr.DrawRectangle(
                    new Pen(color),
                    new Rectangle(
                        Offset(new PointF(rect.X, rect.Y)),
                        new Size((int)(rect.Width * _scaleX),
                            (int)(rect.Height * _scaleX))));
            }

            private void DrawZombies()
            {
                foreach (var zombieVm in _input.Zombies)
                {
                    DrawZombie(zombieVm);
                }
            }

            private void DrawZombie(ZombieDrawInput zombie)
            {
                DrawCone(zombie.Position, zombie.Rotation, ZombieConeSize, ZombieConeRadius,
                    Color.FromArgb(50, 255, 255, 255));
                FillCircleAbsoluteSize(zombie.Position, ZombieCircleRadius, Color.Firebrick);
                if (_input.DrawTargetLines && !zombie.Target.IsEmpty)
                {
                    FillCircleAbsoluteSize(zombie.Target, ZombieTargetRadius, Color.FromArgb(30, 255, 255, 255));
                    _gr.DrawLine(new Pen(Color.FromArgb(30, 255, 255, 255)), Offset(zombie.Position), Offset(zombie.Target));
                }

                if (_input.DrawGroupTargetLines && !zombie.GroupTarget.IsEmpty)
                {
                    FillCircleAbsoluteSize(zombie.GroupTarget, ZombieTargetRadius, Color.FromArgb(30, 255, 255, 150));
                    _gr.DrawLine(new Pen(Color.FromArgb(30, 255, 255, 150)), Offset(zombie.Position), Offset(zombie.GroupTarget));
                }
            }

            private Point Offset(PointF target)
            {
                return new Point((int)((target.X - _offsetX) * _scaleX), (int)((target.Y - _offsetY) * _scaleY));
            }

            private float AdjustRotation(float f)
            {
                return 90 - f;
            }

            private void FillCircleAbsoluteSize(PointF pt, int size, Color color)
            {
                var target = Offset(pt);
                _gr.FillEllipse(
                    new SolidBrush(color),
                    target.X - size / 2,
                    target.Y - size / 2,
                    size,
                    size);
            }

            private void DrawCircle(PointF pt, int size, Color color)
            {
                var target = Offset(pt);
                var sizeX = size * _scaleX;
                var sizeY = size * _scaleY;
                _gr.DrawEllipse(
                    new Pen(color),
                    target.X - sizeX / 2,
                    target.Y - sizeY / 2,
                    sizeX,
                    sizeY);
            }

            private void DrawCone(PointF pt, float rotation, int size, byte radius, Color color)
            {
                var pos = Offset(pt);
                _gr.FillPie(
                    new SolidBrush(color),
                    new Rectangle(
                        new Point(pos.X - size / 2, pos.Y - size / 2), 
                        new Size(size, size)), 
                    AdjustRotation(rotation) - (radius / 2f), 
                    radius);
            }
        }
        
        public BitmapImage Draw(PlayerZoneDrawInput input)
        {
            using var drawer = new DrawPass(input);
            return drawer.Draw();
        }
    }
}