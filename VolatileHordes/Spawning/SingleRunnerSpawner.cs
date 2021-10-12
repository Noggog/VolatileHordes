using System.Threading.Tasks;
using VolatileHordes.AiPackages;
using VolatileHordes.Control;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public class SingleRunnerSpawner
    {
        private readonly ZombieGroupManager _groupManager;
        private readonly RunnerAiPackage _runnerAiPackage;
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieControl _control;
        private readonly ZombieCreator _zombieCreator;

        public SingleRunnerSpawner(
            ZombieGroupManager groupManager,
            RunnerAiPackage runnerAiPackage,
            SpawningPositions spawningPositions,
            ZombieControl control,
            ZombieCreator zombieCreator)
        {
            _groupManager = groupManager;
            _runnerAiPackage = runnerAiPackage;
            _spawningPositions = spawningPositions;
            _control = control;
            _zombieCreator = zombieCreator;
        }
        
        public async Task Spawn(bool nearPlayer = false)
        {
            var spawnTarget = _spawningPositions.GetRandomTarget(nearPlayer);
            if (spawnTarget == null)
            {
                Logger.Warning("Could not find location to spawn single runner");
                return;
            }

            var targetPos = _spawningPositions.GetRandomPosition(spawnTarget.Player.SpawnRectangle);
            if (targetPos == null)
            {
                Logger.Warning("Could not find target position");
                return;
            }
            
            using var groupSpawn = _groupManager.NewGroup(_runnerAiPackage);
            await _zombieCreator.CreateZombie(spawnTarget.SpawnPoint.ToPoint(), groupSpawn.Group);
            
            _control.SendGroupTowards(groupSpawn.Group, targetPos.Value.ToPoint(), withTargetRandomness: false);
        }
    }
}