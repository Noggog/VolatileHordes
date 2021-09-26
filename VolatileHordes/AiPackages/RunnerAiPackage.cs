using VolatileHordes.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.AiPackages
{
    public class RunnerAiPackage : IAiPackage
    {
        private readonly RunnerControl _runnerControl;
        public AiPackageEnum TypeEnum => AiPackageEnum.Runner;

        public RunnerAiPackage(RunnerControl runnerControl)
        {
            _runnerControl = runnerControl;
        }
        
        public void ApplyTo(ZombieGroup group)
        {
            _runnerControl.ApplyTo(group, 10, 80)
                .DisposeWith(group);
        }
    }
}