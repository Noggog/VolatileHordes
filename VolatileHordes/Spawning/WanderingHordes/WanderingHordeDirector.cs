using System;
using System.Reactive.Linq;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordeDirector
    {
        private readonly SingleTracker _singleTracker;

        public WanderingHordeDirector(SingleTracker singleTracker)
        {
            _singleTracker = singleTracker;
        }
        
        public void SpawnHorde()
        {
            TimeManager.Instance.Interval(TimeSpan.FromSeconds(2))
                .Take(10)
                .Subscribe(_ =>
                {
                    _singleTracker.SpawnSingle();
                });
        }
    }
}