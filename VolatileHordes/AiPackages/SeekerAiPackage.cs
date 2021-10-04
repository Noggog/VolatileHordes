using System;
using VolatileHordes.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.AiPackages
{
    public class SeekerAiPackage : IAiPackage
    {
        private readonly PlayerSeekerControl _seekerControl;
        public AiPackageEnum TypeEnum => AiPackageEnum.Seeker;

        public SeekerAiPackage(PlayerSeekerControl seekerControl)
        {
            _seekerControl = seekerControl;
        }
        
        public void ApplyTo(ZombieGroup group)
        {
            _seekerControl.ApplyTo(group, TimeSpan.FromSeconds(10))
                .Subscribe()
                .DisposeWith(group);
        }
    }
}