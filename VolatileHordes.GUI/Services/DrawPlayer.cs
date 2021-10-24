using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;
using VolatileHordes.GUI.Extensions;
using VolatileHordes.GUI.ViewModels;

namespace VolatileHordes.GUI.Services
{
    public record DrawInput(PlayerVm Player, ushort Size, ZombieVm[] Zombies);
    
    public class DrawPlayerZone
    {
        private const byte PlayerConeSize = 50;
        private const byte PlayerConeRadius = 70;
        private const byte PlayerCircleRadius = 4;

        class DrawPass : IDisposable
        {
            private readonly DrawInput _input;
            private readonly int _half;
            private readonly Bitmap _bitmap;
            private readonly Graphics _gr;
            private readonly float _offsetX;
            private readonly float _offsetY;
            
            public DrawPass(DrawInput input)
            {
                _input = input;
                _half = input.Size / 2;
                _bitmap = new Bitmap(input.Size, input.Size);
                _gr = Graphics.FromImage(_bitmap);
                _offsetX = input.Player.SpawnRectangle.Left;
                _offsetY = input.Player.SpawnRectangle.Top;
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
                return _bitmap.ToBitmapImage();
            }

            private void DrawPlayer()
            {
                _gr.FillPie(
                    new SolidBrush(Color.FromArgb(50, 255, 255, 255)),
                    new Rectangle(
                        new Point(_half - PlayerConeSize / 2, _half - PlayerConeSize / 2), 
                        new Size(PlayerConeSize, PlayerConeSize)), 
                    _input.Player.Rotation - (PlayerConeRadius / 2f), PlayerConeRadius);
                _gr.FillEllipse(new SolidBrush(Color.Teal), _half - PlayerCircleRadius / 2, _half - PlayerCircleRadius / 2, PlayerCircleRadius, PlayerCircleRadius);
            }

            private void DrawZombies()
            {
                foreach (var zombieVm in _input.Zombies)
                {
                    DrawZombie(zombieVm);
                }
            }

            private void DrawZombie(ZombieVm zvm)
            {
                
            }
        }
        
        public BitmapImage Draw(DrawInput input)
        {
            using var drawer = new DrawPass(input);
            return drawer.Draw();
        }
    }
}