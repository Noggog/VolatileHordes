using System;
using VolatileHordes.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.AiPackages
{
    public class CrazyAiPackage : IAiPackage
    {
        private readonly NoiseResponderControlFactory _noiseResponderControlFactory;
        private readonly CrazyControl _crazyControl;
        public AiPackageEnum TypeEnum => AiPackageEnum.Crazy;

        public CrazyAiPackage(
            NoiseResponderControlFactory noiseResponderControlFactory,
            CrazyControl crazyControl)
        {
            _noiseResponderControlFactory = noiseResponderControlFactory;
            _crazyControl = crazyControl;
        }
        
        public void ApplyTo(ZombieGroup group)
        {
            var noiseControl = _noiseResponderControlFactory.Create();
            noiseControl.ApplyTo(group)
                .Subscribe()
                .DisposeWith(group);
            _crazyControl.ApplyTo(group, new TimeRange(TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(15)), 50)
                .Compose(noiseControl.TemporaryShutoffOnNoise(), TimeSpan.FromSeconds(60), nameof(CrazyAiPackage))
                .Subscribe()
                .DisposeWith(group);
        }
    }
}