using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using Splat;
using VolatileHordes.GUI.Extensions;

namespace VolatileHordes.GUI.Services
{
    public record PlayerDrawInput(RectangleF Rectangle, float Rotation);
    public record ZombieDrawInput(PointF Position, PointF Target);
    public record DrawInput(PlayerDrawInput Player, ushort Size, ZombieDrawInput[] Zombies);
    
    public class DrawPlayerZone
    {
        private const byte PlayerConeSize = 50;
        private const byte PlayerConeRadius = 70;
        private const byte PlayerCircleRadius = 4;
        private const byte ZombieCircleRadius = 4;

        class DrawPass : IDisposable
        {
            private readonly DrawInput _input;
            private readonly int _half;
            private readonly Bitmap _bitmap;
            private readonly Graphics _gr;
            private readonly float _offsetX;
            private readonly float _offsetY;
            private readonly float _scaleX;
            private readonly float _scaleY;
            
            public DrawPass(DrawInput input)
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
                var pos = Offset(_input.Player.Rectangle.Center());
                _gr.FillPie(
                    new SolidBrush(Color.FromArgb(50, 255, 255, 255)),
                    new Rectangle(
                        new Point(pos.X - PlayerConeSize / 2, pos.Y - PlayerConeSize / 2), 
                        new Size(PlayerConeSize, PlayerConeSize)), 
                    AdjustRotation(_input.Player.Rotation) - (PlayerConeRadius / 2f), 
                    PlayerConeRadius);
                _gr.FillEllipse(
                    new SolidBrush(Color.Teal), 
                    pos.X - PlayerCircleRadius / 2, 
                    pos.Y - PlayerCircleRadius / 2, PlayerCircleRadius, PlayerCircleRadius);
            }

            private Point Offset(PointF target)
            {
                return new Point((int)((target.X - _offsetX) * _scaleX), (int)((target.Y - _offsetY) * _scaleY));
            }

            private float AdjustRotation(float f)
            {
                return 90 - f;
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
                var pos = Offset(zombie.Position);
                _gr.FillEllipse(
                    new SolidBrush(Color.Firebrick),
                    pos.X - ZombieCircleRadius / 2,
                    pos.Y - ZombieCircleRadius / 2,
                    ZombieCircleRadius,
                    ZombieCircleRadius);
            }
        }
        
        public BitmapImage Draw(DrawInput input)
        {
            using var drawer = new DrawPass(input);
            return drawer.Draw();
        }
    }
}