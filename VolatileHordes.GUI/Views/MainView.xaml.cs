using System.Reactive.Disposables;
using Noggog.WPF;
using ReactiveUI;
using VolatileHordes.GUI.ViewModels;

namespace VolatileHordes.GUI.Views
{
    public class MainViewBase : NoggogUserControl<MainVm> { }
    
    public partial class MainView : MainViewBase
    {
        public MainView()
        {
            InitializeComponent();
            this.WhenActivated(dispose =>
            {
                this.WhenAnyValue(x => x.ViewModel!.Connection)
                    .BindTo(this, x => x.ConnectionView.DataContext)
                    .DisposeWith(dispose);
                this.WhenAnyValue(x => x.ViewModel!.Player)
                    .BindTo(this, x => x.PlayerView.DataContext)
                    .DisposeWith(dispose);
            });
        }
    }
}