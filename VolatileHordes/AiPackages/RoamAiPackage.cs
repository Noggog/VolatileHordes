using System;
using VolatileHordes.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.AiPackages
{
    public class RoamAiPackage : IAiPackage
    {
        private readonly FidgetRoam _fidget;
        private readonly NoiseResponderControlFactory _noiseResponderControlFactory;
        private readonly RoamFarOccasionally _roamFarOccasionally;
        private readonly LuckyPlayerRetarget _luckyPlayerRetarget;

        public RoamAiPackage(
            FidgetRoam fidget,
            NoiseResponderControlFactory noiseResponderControlFactory,
            RoamFarOccasionally roamFarOccasionally,
            LuckyPlayerRetarget luckyPlayerRetarget)
        {
            _fidget = fidget;
            _noiseResponderControlFactory = noiseResponderControlFactory;
            _roamFarOccasionally = roamFarOccasionally;
            _luckyPlayerRetarget = luckyPlayerRetarget;
        }

        public AiPackageEnum TypeEnum => AiPackageEnum.Roaming;

        public void ApplyTo(ZombieGroup group)
        {
            _luckyPlayerRetarget.ApplyTo(group, out var luckyOccurred)
                .Subscribe()
                .DisposeWith(group);
            var noiseControl = _noiseResponderControlFactory.Create();
            noiseControl.ApplyTo(group)
                .Subscribe()
                .DisposeWith(group);
            _fidget.ApplyTo(group, interrupt: luckyOccurred)
                .Compose(noiseControl.TemporaryShutoffOnNoise(), TimeSpan.FromSeconds(60), nameof(RoamAiPackage))
                .Subscribe()
                .DisposeWith(group);
            _roamFarOccasionally.ApplyTo(group, interrupt: luckyOccurred)
                .Compose(noiseControl.TemporaryShutoffOnNoise(), TimeSpan.FromSeconds(60), nameof(RoamAiPackage))
                .Subscribe()
                .DisposeWith(group);
        }
    }
}