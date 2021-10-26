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
                this.Bind(ViewModel, x => x.Settings.DrawTargets, x => x.TargetsBox.IsChecked)
                    .DisposeWith(dispose);
                this.Bind(ViewModel, x => x.Settings.DrawGroupTargets, x => x.GroupTargetsBox.IsChecked)
                    .DisposeWith(dispose);
                this.OneWayBind(ViewModel, x => x.Players, x => x.PlayersChecklist.ItemsSource)
                    .DisposeWith(dispose);
                this.OneWayBind(ViewModel, x => x.DisplayPlayers, x => x.PlayerViews.ItemsSource)
                    .DisposeWith(dispose);
            });
        }
    }
}