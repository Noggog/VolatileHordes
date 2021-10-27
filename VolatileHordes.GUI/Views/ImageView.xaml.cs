using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Media;
using Noggog.WPF;
using ReactiveUI;
using VolatileHordes.GUI.ViewModels;

namespace VolatileHordes.GUI.Views
{
    public class ImageViewBase : NoggogUserControl<IImageVm> { }
    
    public partial class ImageView
    {
        public ImageView()
        {
            InitializeComponent();
            this.WhenActivated(dispose =>
            {
                this.WhenAnyValue(x => x.ViewModel!.Bitmap)
                    .Select(x => x as ImageSource)
                    .BindTo(this, x => x.Image.Source)
                    .DisposeWith(dispose);
                this.Bind(ViewModel, x => x.Title, x => x.Title.Text)
                    .DisposeWith(dispose);
            });
        }
    }
}