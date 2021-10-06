using System;
using VolatileHordes.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.AiPackages
{
    public class FidgetForwardAIPackage : IAiPackage
    {
        private readonly LuckyPlayerRetarget _luckyPlayerRetarget;
        private readonly FidgetForward _fidgetForward;

        public FidgetForwardAIPackage(
            LuckyPlayerRetarget luckyPlayerRetarget,
            FidgetForward fidgetForward)
        {
            _luckyPlayerRetarget = luckyPlayerRetarget;
            _fidgetForward = fidgetForward;
        }

        public AiPackageEnum TypeEnum => AiPackageEnum.FidgetForward;

        public void ApplyTo(ZombieGroup group)
        {
            _luckyPlayerRetarget.ApplyTo(group, out var luckyOccurred)
                .DisposeWith(group);
            _fidgetForward.ApplyTo(group, luckyOccurred)
                .DisposeWith(group);
        }
    }
}