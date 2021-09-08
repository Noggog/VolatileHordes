using System;
using System.Reactive.Linq;
using VolatileHordes.Randomization;
using VolatileHordes.Spawning;

namespace VolatileHordes
{
    public class HordeManager
    {
        public static readonly HordeManager Instance = new();
        
        public void Init()
        {
            // TimeManager.Instance.Interval(TimeSpan.FromSeconds(30))
            //     .Subscribe(_ =>
            //     {
            //         Logger.Info("Spawning a zombie");
            //     });
        }
    }
}