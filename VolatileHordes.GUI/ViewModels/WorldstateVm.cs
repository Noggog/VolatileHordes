using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
        
        [Reactive] public ushort NoiseRadius { get; set; }
        
        [Reactive] public Rectangle WorldRect { get; set; }
        
        public WorldstateVm(
            PlayerVm.Factory pvmFactory,
            AllocationVm.Factory allocationVmFactory,
            ConnectionVm connectionVm)
        {
            _pvmFactory = pvmFactory;
            AllocationVm = allocationVmFactory();
            connectionVm.WhenAnyValue(x => x.Client)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x =>
                {
                    if (x == null) return Observable.Return<State?>(null);
                    return x.States.Catch<State, Exception>((ex) => Observable.Empty<State>());
                })
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
            AllocationVm.ChunkSize = state?.AllocationState.ChunkSize ?? 100;
            AllocationVm.Set(state?.AllocationState.Buckets);
            NoiseRadius = state?.NoiseRadius ?? 0;
            WorldRect = state?.WorldRect ?? new Rectangle(-4096, -4096, 8192, 8192);
        }
    }
}