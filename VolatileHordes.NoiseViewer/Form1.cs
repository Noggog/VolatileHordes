using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using VolatileHordes.NoiseViewer.Simulations;

namespace VolatileHordes.NoiseViewer
{
    public partial class Form1 : Form
    {
        public const int WorldRadius = 125;
        public const int RadiusSize = 100;
        public const int ScaleAmount = 2;
        public const int ImageSize = WorldRadius * 2 * ScaleAmount;
        public const int HalfImageSize = ImageSize / 2;
        
        public Form1()
        {
            InitializeComponent();
            Observable.Merge(
                    Observable.FromEventPattern<EventHandler, EventArgs>(
                            h => this.ResetButton.Click += h,
                            h => this.ResetButton.Click -= h)
                        .Unit()
                        .StartWith(Unit.Default)
                        .Do(_ => SimulationContainer.Simulation.Reset()),
                    Observable.FromEventPattern<EventHandler, EventArgs>(
                            h => this.NoiseButton.Click += h,
                            h => this.NoiseButton.Click -= h)
                        .Unit()
                        .Do(_ => SimulationContainer.NoiseManager.CauseNoise()))
                .Select(_ => GetBitmap())
                .DisposeOld()
                .Subscribe(x =>
                {
                    this.DisplayBox.Image = x;
                    this.DisplayBox.Width = x.Width;
                    this.DisplayBox.Visible = true;
                    this.DisplayBox.Height = x.Height;
                });
        }

        public int Reposition(int i)
        {
            return Scale(i + WorldRadius);
        }

        public int Scale(int i)
        {
            return i * ScaleAmount;
        }

        private Bitmap GetBitmap()
        {
            var bm = new Bitmap(ImageSize, ImageSize);
            using System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bm);
            gr.SmoothingMode = SmoothingMode.None;
            gr.Clear(System.Drawing.Color.Black);

            // Draw noise radius
            DrawCircle(gr, new SolidBrush(Color.FromArgb(15, 15, 15)), new Point(0, 0), RadiusSize);
            // Draw player circle
            DrawCircle(gr, new SolidBrush(Color.CadetBlue), new PointF(0, 0), 2);
            
            // Draw zombie circles
            foreach (var simulationZombieGroup in SimulationContainer.Simulation.ZombieGroups)
            {
                var pt = simulationZombieGroup.Target!;
                DrawCircle(gr, new SolidBrush(Color.Firebrick), new PointF((int)pt.Value.X, (int)pt.Value.Y), 2);
            }

            return bm;
        }

        private void DrawCircle(Graphics gr, Brush brush, PointF pt, int radius)
        {
            gr.FillEllipse(brush, Reposition((int)(pt.X - radius)), Reposition((int)(pt.Y - radius)), Scale(radius * 2), Scale(radius * 2));
        }
    }
}