using System.Reactive.Disposables;
using System.Windows.Controls;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using VolatileHordes.GUI.ViewModels;

namespace VolatileHordes.GUI.Views
{
    public class LimitsViewBase : NoggogUserControl<LimitsVm> { }
    
    public partial class LimitsView
    {
        public LimitsView()
        {
            InitializeComponent();
            this.WhenActivated(dispose =>
            {
                this.Bind(ViewModel, x => x.GameMaximum, x => x.MaxDesiredBar.Maximum)
                    .DisposeWith(dispose);
                this.Bind(ViewModel, x => x.GameMaximum, x => x.CurrentZombiesBar.Maximum)
                    .DisposeWith(dispose);
                
                this.Bind(ViewModel, x => x.DesiredMaximum, x => x.MaxDesiredBar.Value)
                    .DisposeWith(dispose);
                this.Bind(ViewModel, x => x.CurrentNumber, x => x.CurrentZombiesBar.Value)
                    .DisposeWith(dispose);
            });
        }
    }
}