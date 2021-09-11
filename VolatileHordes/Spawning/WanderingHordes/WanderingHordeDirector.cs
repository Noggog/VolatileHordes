using System;
using System.Reactive.Linq;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordeDirector
    {
        public static readonly WanderingHordeDirector Instance = new();

        public void SpawnHorde()
        {
            ZombieCreator.Instance.PrintZombieStats();
            TimeManager.Instance.Interval(TimeSpan.FromSeconds(2))
                .Take(10)
                .Subscribe(_ =>
                {
                    SingleTracker.Instance.SpawnSingle();
                });
        }
    }
}