using System.Reactive.Disposables;
using Noggog.WPF;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Media;
using VolatileHordes.GUI.ViewModels;

namespace VolatileHordes.GUI.Views
{
    public class PlayerViewBase : NoggogUserControl<PlayerDisplayVm> { }
    
    public partial class PlayerView
    {
        public PlayerView()
        {
            InitializeComponent();
            this.WhenActivated(dispose =>
            {
                this.WhenAnyValue(x => x.ViewModel!.Bitmap)
                    .Select(x => x as ImageSource)
                    .BindTo(this, x => x.Image.Source)
                    .DisposeWith(dispose);
            });
        }
    }
}