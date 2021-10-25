using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using Noggog;
using Noggog.WPF;
using ReactiveUI;

namespace VolatileHordes.GUI.ViewModels
{
    public class MainVm : ViewModel
    {
        public ConnectionVm Connection { get; }

        private readonly ObservableAsPropertyHelper<PlayerDisplayVm?> _Player;
        public PlayerDisplayVm? Player => _Player.Value;
        
        public MainSettingsVm Settings { get; }

        public MainVm(
            ConnectionVm connectionVm,
            PlayerDisplayVm.Factory pvmFactory,
            MainSettingsVm settingsVm,
            WorldstateVm worldstateVm)
        {
            Connection = connectionVm;
            Settings = settingsVm;

            _Player = worldstateVm.Players.Connect()
                .QueryWhenChanged(x => x.Items.FirstOrDefault())
                .Select(x => x == null ? null : pvmFactory(x))
                .DisposePrevious()
                .ToGuiProperty(this, nameof(Player), default);
        }

        public void Init()
        {
        }
    }
}