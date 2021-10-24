using System;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using Noggog.WPF;
using ReactiveUI;
using VolatileHordes.Dto;
using VolatileHordes.GUI.Extensions;

namespace VolatileHordes.GUI.ViewModels
{
    public class MainVm : ViewModel
    {
        public ConnectionVm Connection { get; }

        private SourceCache<PlayerVm, int> _players = new(x => x.EntityId);

        private SourceCache<ZombieGroupVm, int> _zombies = new(x => x.Id);

        private readonly ObservableAsPropertyHelper<PlayerVm?> _Player;
        public PlayerVm? Player => _Player.Value;

        public MainVm(ConnectionVm connectionVm)
        {
            Connection = connectionVm;
            Connection.WhenAnyValue(x => x.Client)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x => x?.States ?? Observable.Empty<State>())
                .Switch()
                .Subscribe(state =>
                {
                    _players.AbsorbIn(
                        state.Players, 
                        o => o.EntityId, 
                        (k) => new PlayerVm(k),
                        (vm, dto) => vm.Absorb(dto));
                    _zombies.AbsorbIn(
                        state.ZombieGroups, 
                        o => o.Id, 
                        (k) => new ZombieGroupVm(k),
                        (vm, dto) => vm.Absorb(dto));
                });

            _Player = _players.Connect()
                .QueryWhenChanged(x => x.Items.FirstOrDefault())
                .ToGuiProperty(this, nameof(Player), default);
        }

        public void Init()
        {
        }
    }
}