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

        public RoamAiPackage(
            FidgetRoam fidget,
            RoamFarOccasionally roamFarOccasionally)
        {
            _fidget = fidget;
            _roamFarOccasionally = roamFarOccasionally;
        }

        public AiPackageEnum TypeEnum => AiPackageEnum.Roaming;

        public void ApplyTo(ZombieGroup group)
        {
            _fidget.ApplyTo(group)
                .DisposeWith(group);
            _roamFarOccasionally.ApplyTo(group)
                .DisposeWith(group);
        }
    }
}