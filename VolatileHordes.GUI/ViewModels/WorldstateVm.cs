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
        private readonly PlayerVm.Factory _pvmFactory;
        public LimitsVm Limits { get; } = new();
        
        private SourceCache<PlayerVm, int> _players = new(x => x.EntityId);
        public IObservableCache<PlayerVm, int> Players => _players;

        private SourceCache<ZombieGroupVm, int> _zombieGroups = new(x => x.Id);
        public IObservableCache<ZombieGroupVm, int> ZombieGroups => _zombieGroups;

        public AllocationVm AllocationVm { get; }
        
        public WorldstateVm(
            PlayerVm.Factory pvmFactory,
            AllocationVm.Factory allocationVmFactory,
            ConnectionVm connectionVm)
        {
            _pvmFactory = pvmFactory;
            AllocationVm = allocationVmFactory();
            connectionVm.WhenAnyValue(x => x.Client)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x => x?.States ?? Observable.Return<State?>(null))
                .Switch()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(Absorb);
        }

        private void Absorb(State? state)
        {
            _players.AbsorbIn(
                state?.Players ?? Enumerable.Empty<PlayerDto>(), 
                o => o.EntityId, 
                (k) => _pvmFactory(k),
                (vm, dto) => vm.Absorb(dto));
            Limits.AbsorbIn(state?.Limits);
            _zombieGroups.AbsorbIn(
                state?.ZombieGroups ?? Enumerable.Empty<ZombieGroupDto>(), 
                o => o.Id, 
                (k) => new ZombieGroupVm(k),
                (vm, dto) => vm.Absorb(dto));
            AllocationVm.Set(state?.AllocationState.Buckets);
        }
    }
}