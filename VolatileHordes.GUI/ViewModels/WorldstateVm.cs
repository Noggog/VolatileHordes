using System;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using VolatileHordes.Dto;
using VolatileHordes.GUI.Extensions;

namespace VolatileHordes.GUI.ViewModels
{
    public class WorldstateVm
    {
        public LimitsVm Limits { get; } = new();
        
        private SourceCache<PlayerVm, int> _players = new(x => x.EntityId);
        public IObservableCache<PlayerVm, int> Players => _players;

        private SourceCache<ZombieGroupVm, int> _zombieGroups = new(x => x.Id);
        public IObservableCache<ZombieGroupVm, int> ZombieGroups => _zombieGroups;
        
        public WorldstateVm(
            PlayerVm.Factory pvmFactory,
            ConnectionVm connectionVm)
        {
            connectionVm.WhenAnyValue(x => x.Client)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x => x?.States ?? Observable.Return<State?>(null))
                .Switch()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(state =>
                {
                    _players.AbsorbIn(
                        state?.Players ?? Enumerable.Empty<PlayerDto>(), 
                        o => o.EntityId, 
                        (k) => pvmFactory(k),
                        (vm, dto) => vm.Absorb(dto));
                    Limits.AbsorbIn(state?.Limits);
                    _zombieGroups.AbsorbIn(
                        state?.ZombieGroups ?? Enumerable.Empty<ZombieGroupDto>(), 
                        o => o.Id, 
                        (k) => new ZombieGroupVm(k),
                        (vm, dto) => vm.Absorb(dto));
                });
        }
    }
}