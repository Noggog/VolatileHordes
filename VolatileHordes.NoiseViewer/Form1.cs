using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VolatileHordes.Utility;

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
            Observable.FromEventPattern<EventHandler, EventArgs>(
                    h => this.ResetButton.Click += h,
                    h => this.ResetButton.Click -= h)
                .Unit()
                .StartWith(Unit.Default)
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

            gr.FillEllipse(new SolidBrush(Color.FromArgb(15, 15, 15)), Reposition(0 - RadiusSize), Reposition(0 - RadiusSize), Scale(RadiusSize * 2), Scale(RadiusSize * 2));

            return bm;
        }
    }
}