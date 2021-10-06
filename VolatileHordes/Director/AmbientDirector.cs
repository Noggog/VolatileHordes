using System;
using VolatileHordes.AiPackages;
using VolatileHordes.Probability;
using VolatileHordes.Tracking;

namespace VolatileHordes.Director
{
    public class AmbientDirector
    {
        private readonly RandomSource _randomSource;
        private readonly CrazyAiPackage _crazyAiPackage;

        public AmbientDirector(
            DirectorSwitch directorSwitch,
            RandomSource randomSource,
            CrazyAiPackage crazyAiPackage,
            RunnerAiPackage runnerAiPackage,
            AmbientZombieManager ambientZombieManager)
        {
            _randomSource = randomSource;
            _crazyAiPackage = crazyAiPackage;
            ambientZombieManager.GroupTracked
                .FlowSwitch(directorSwitch.Enabled)
                .Subscribe(AmbientZombieSpawned);
        }

        private void AmbientZombieSpawned(ZombieGroup group)
        {
            if (_randomSource.Chance(0.25f))
            {
                group.ApplyPackage(_crazyAiPackage);
            }
        }
    }
}