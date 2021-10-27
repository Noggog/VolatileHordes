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
        PlayerDrawInput Player,
        ushort Size, 
        ZombieDrawInput[] Zombies,
        bool DrawTargetLines,
        bool DrawGroupTargetLines);
    
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
            private readonly int _half;
            private readonly Bitmap _bitmap;
            private readonly Graphics _gr;
            private readonly float _offsetX;
            private readonly float _offsetY;
            private readonly float _scaleX;
            private readonly float _scaleY;
            
            public DrawPass(PlayerZoneDrawInput input)
            {
                _input = input;
                _half = input.Size / 2;
                _bitmap = new Bitmap(input.Size, input.Size);
                _gr = Graphics.FromImage(_bitmap);
                _offsetX = input.Player.Rectangle.Left;
                _offsetY = input.Player.Rectangle.Top;
                _scaleX = input.Size / input.Player.Rectangle.Width;
                _scaleY = input.Size / input.Player.Rectangle.Height;
            }

            public void Dispose()
            {
                _gr.Dispose();
                _bitmap.Dispose();
            }

            public BitmapImage Draw()
            {
                _gr.Clear(Color.Black);
                DrawPlayer();
                DrawZombies();
                _bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
                return _bitmap.ToBitmapImage();
            }

            private void DrawPlayer()
            {
                if (_input.Player.SpawnRectangle.IsEmpty) return;
                var center = _input.Player.SpawnRectangle.Center();
                DrawCone(center, _input.Player.Rotation, PlayerConeSize, PlayerConeRadius,
                    Color.FromArgb(50, 255, 255, 255));
                _gr.DrawRectangle(
                    new Pen(Color.FromArgb(15, 15, 15)),
                    new Rectangle(
                        Offset(new PointF(_input.Player.SpawnRectangle.X, _input.Player.SpawnRectangle.Y)),
                        new Size((int)(_input.Player.SpawnRectangle.Width * _scaleX), (int)(_input.Player.SpawnRectangle.Height * _scaleX))));
                DrawCircle(center, PlayerCircleRadius, Color.Teal);
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
                DrawCircle(zombie.Position, ZombieCircleRadius, Color.Firebrick);
                if (_input.DrawTargetLines && !zombie.Target.IsEmpty)
                {
                    DrawCircle(zombie.Target, ZombieTargetRadius, Color.FromArgb(30, 255, 255, 255));
                    _gr.DrawLine(new Pen(Color.FromArgb(30, 255, 255, 255)), Offset(zombie.Position), Offset(zombie.Target));
                }

                if (_input.DrawGroupTargetLines && !zombie.GroupTarget.IsEmpty)
                {
                    DrawCircle(zombie.GroupTarget, ZombieTargetRadius, Color.FromArgb(30, 255, 255, 150));
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

            private void DrawCircle(PointF pt, int size, Color color)
            {
                var target = Offset(pt);
                _gr.FillEllipse(
                    new SolidBrush(color),
                    target.X - size / 2,
                    target.Y - size / 2,
                    size,
                    size);
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