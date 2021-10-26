using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using Noggog.WPF;
using ReactiveUI;
using VolatileHordes.GUI.ViewModels;

namespace VolatileHordes.GUI.Views
{
    public class ConnectionViewBase : NoggogUserControl<ConnectionVm> { }
    
    public partial class ConnectionView : ConnectionViewBase
    {
        public ConnectionView()
        {
            InitializeComponent();
            this.WhenActivated(dispose =>
            {
                this.WhenAnyValue(x => x.ViewModel!.Connected)
                    .Select(x => x ? Visibility.Hidden : Visibility.Visible)
                    .BindTo(this, x => x.ConnectButton.Visibility)
                    .DisposeWith(dispose);
                this.OneWayBind(ViewModel, x => x.ConnectCommand, x => x.ConnectButton.Command)
                    .DisposeWith(dispose);
                this.OneWayBind(ViewModel, x => x.DisconnectCommand, x => x.DisconnectButton.Command)
                    .DisposeWith(dispose);
                this.WhenAnyValue(x => x.ViewModel!.Connected)
                    .Select(x => x ? Visibility.Visible : Visibility.Hidden)
                    .BindTo(this, x => x.DisconnectButton.Visibility)
                    .DisposeWith(dispose);
                this.Bind(ViewModel, x => x.First, x => x.FirstInput.Value)
                    .DisposeWith(dispose);
                this.Bind(ViewModel, x => x.Second, x => x.SecondInput.Value)
                    .DisposeWith(dispose);
                this.Bind(ViewModel, x => x.Third, x => x.ThirdInput.Value)
                    .DisposeWith(dispose);
                this.Bind(ViewModel, x => x.Fourth, x => x.FourthInput.Value)
                    .DisposeWith(dispose);
                this.Bind(ViewModel, x => x.Port, x => x.PortInput.Value,
                        vmToViewConverter: x => (int)x,
                        viewToVmConverter: x => (ushort)(x ?? 0))
                    .DisposeWith(dispose);
            });
        }
    }
}