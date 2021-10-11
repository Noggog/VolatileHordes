using System;
using VolatileHordes.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.AiPackages
{
    public class FidgetForwardAIPackage : IAiPackage
    {
        private readonly LuckyPlayerRetarget _luckyPlayerRetarget;
        private readonly FidgetForwardControl _fidgetForwardControl;

        public FidgetForwardAIPackage(
            LuckyPlayerRetarget luckyPlayerRetarget,
            FidgetForwardControl fidgetForwardControl)
        {
            _luckyPlayerRetarget = luckyPlayerRetarget;
            _fidgetForwardControl = fidgetForwardControl;
        }

        public AiPackageEnum TypeEnum => AiPackageEnum.FidgetForward;

        public void ApplyTo(ZombieGroup group)
        {
            _luckyPlayerRetarget.ApplyTo(group, out var luckyOccurred)
                .Subscribe()
                .DisposeWith(group);
            _fidgetForwardControl.ApplyTo(group, luckyOccurred)
                .DisposeWith(group);
        }
    }
}