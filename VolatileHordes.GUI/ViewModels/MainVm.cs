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

        public MainVm(
            ConnectionVm connectionVm,
            WorldstateVm worldstateVm)
        {
            Connection = connectionVm;

            _Player = worldstateVm.Players.Connect()
                .QueryWhenChanged(x => x.Items.FirstOrDefault())
                .Select(x => x == null ? null : new PlayerDisplayVm(x))
                .DisposePrevious()
                .ToGuiProperty(this, nameof(Player), default);
        }

        public void Init()
        {
        }
    }
}