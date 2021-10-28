using System;
using VolatileHordes.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.AiPackages
{
    public class RunnerAiPackage : IAiPackage
    {
        private readonly NoiseResponderControlFactory _noiseResponderControlFactory;
        private readonly RunnerControl _runnerControl;
        public AiPackageEnum TypeEnum => AiPackageEnum.Runner;

        public RunnerAiPackage(
            NoiseResponderControlFactory noiseResponderControlFactory,
            RunnerControl runnerControl)
        {
            _noiseResponderControlFactory = noiseResponderControlFactory;
            _runnerControl = runnerControl;
        }
        
        public void ApplyTo(ZombieGroup group)
        {
            var noiseControl = _noiseResponderControlFactory.Create();
            noiseControl.ApplyTo(group)
                .Subscribe()
                .DisposeWith(group);
            _runnerControl.ApplyTo(group, 10, 80)
                .Compose(noiseControl.TemporaryShutoffOnNoise(), TimeSpan.FromSeconds(60), nameof(CrazyAiPackage))
                .Subscribe()
                .DisposeWith(group);
        }
    }
}