using System;
using VolatileHordes.AiPackages;
using VolatileHordes.Core.Models;
using VolatileHordes.Probability;
using VolatileHordes.Tracking;

namespace VolatileHordes.Director
{
    public class AmbientDirector
    {
        private readonly RandomSource _randomSource;
        private readonly ProbabilityList<IAiPackage?> _packages = new();

        public AmbientDirector(
            DirectorSwitch directorSwitch,
            RandomSource randomSource,
            CrazyAiPackage crazyAiPackage,
            AmbientAiPackage ambientAi,
            RunnerAiPackage runnerAiPackage,
            ZombieGroupManager groupManager)
        {
            _randomSource = randomSource;
            _packages.Add(crazyAiPackage, new UDouble(0.25d));
            _packages.Add(runnerAiPackage, new UDouble(0.25d));
            _packages.Add(ambientAi, new UDouble(1d));
            groupManager.AmbientGroupTracked
                .FlowSwitch(directorSwitch.Enabled)
                .Subscribe(AmbientZombieSpawned);
        }

        private void AmbientZombieSpawned(ZombieGroup group)
        {
            Logger.Verbose("Analyzing which package to give ambient zombie {0}", group.Id);
            var package = _packages.Get(_randomSource);
            if (package == null)
            {
                Logger.Verbose("Applying nothing to ambient zombie {0}", group.Id);
                group.Dispose();
            }
            else
            {
                Logger.Debug("Applying {0} to ambient zombie {1}", package.GetType().Name, group.Id);
                group.ApplyPackage(package);
            }
        }
    }
}