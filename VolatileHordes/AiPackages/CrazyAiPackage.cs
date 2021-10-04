using System;
using VolatileHordes.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.AiPackages
{
    public class CrazyAiPackage : IAiPackage
    {
        private readonly CrazyControl _crazyControl;
        public AiPackageEnum TypeEnum => AiPackageEnum.Crazy;

        public CrazyAiPackage(CrazyControl crazyControl)
        {
            _crazyControl = crazyControl;
        }
        
        public void ApplyTo(ZombieGroup group)
        {
            _crazyControl.ApplyTo(group, new TimeRange(TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(15)), 50)
                .Subscribe()
                .DisposeWith(group);
        }
    }
}