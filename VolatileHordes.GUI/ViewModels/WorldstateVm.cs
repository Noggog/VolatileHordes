using System;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using VolatileHordes.Dto;
using VolatileHordes.GUI.Extensions;

namespace VolatileHordes.GUI.ViewModels
{
    public class WorldstateVm
    {
        private SourceCache<PlayerVm, int> _players = new(x => x.EntityId);
        public IObservableCache<PlayerVm, int> Players => _players;

        private SourceCache<ZombieGroupVm, int> _zombieGroups = new(x => x.Id);
        public IObservableCache<ZombieGroupVm, int> ZombieGroups => _zombieGroups;
        
        public WorldstateVm(
            ConnectionVm connectionVm,
            PlayerVm.Factory playerFactory)
        {
            connectionVm.WhenAnyValue(x => x.Client)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x => x?.States ?? Observable.Empty<State>())
                .Switch()
                .Subscribe(state =>
                {
                    _players.AbsorbIn(
                        state.Players, 
                        o => o.EntityId, 
                        (k) => playerFactory(k),
                        (vm, dto) => vm.Absorb(dto));
                    _zombieGroups.AbsorbIn(
                        state.ZombieGroups, 
                        o => o.Id, 
                        (k) => new ZombieGroupVm(k),
                        (vm, dto) => vm.Absorb(dto));
                });
        }
    }
}