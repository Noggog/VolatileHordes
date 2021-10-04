using System;
using VolatileHordes.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.AiPackages
{
    public class RoamAiPackage : IAiPackage
    {
        private readonly FidgetRoam _fidget;
        private readonly RoamFarOccasionally _roamFarOccasionally;
        private readonly LuckyPlayerRetarget _luckyPlayerRetarget;

        public RoamAiPackage(
            FidgetRoam fidget,
            RoamFarOccasionally roamFarOccasionally,
            LuckyPlayerRetarget luckyPlayerRetarget)
        {
            _fidget = fidget;
            _roamFarOccasionally = roamFarOccasionally;
            _luckyPlayerRetarget = luckyPlayerRetarget;
        }

        public AiPackageEnum TypeEnum => AiPackageEnum.Roaming;

        public void ApplyTo(ZombieGroup group)
        {
            _luckyPlayerRetarget.ApplyTo(group, out var luckyOccurred)
                .Subscribe()
                .DisposeWith(group);
            _fidget.ApplyTo(group, interrupt: luckyOccurred)
                .Subscribe()
                .DisposeWith(group);
            _roamFarOccasionally.ApplyTo(group, interrupt: luckyOccurred)
                .Subscribe()
                .DisposeWith(group);
        }
    }
}