using VolatileHordes.AiPackages;
using VolatileHordes.Control;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public class SingleRunnerDirector
    {
        private readonly GroupManager _groupManager;
        private readonly RunnerAiPackage _runnerAiPackage;
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieControl _control;
        private readonly ZombieCreator _zombieCreator;

        public SingleRunnerDirector(
            GroupManager groupManager,
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
        
        public void Spawn(bool nearPlayer = false)
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null)
            {
                Logger.Warning("Could not find location to spawn single runner");
                return;
            }

            var spawnPt = spawnTarget.SpawnPoint.ToPoint();

            var nearestPlayer = _spawningPositions.GetNearestPlayer(spawnTarget.TriggerOrigin);
            if (nearestPlayer == null)
            {
                Logger.Warning("Could not find nearest player");
                return;
            }

            if (nearPlayer)
            {
                var newTarget = _spawningPositions.GetRandomEdgeRangeAwayFrom(spawnTarget.TriggerOrigin, range: 30);
                if (newTarget == null)
                {
                    Logger.Warning("Could not find artificial point near player");
                    return;
                }
                spawnPt = newTarget.Value.ToPoint();
            }

            var targetPos = _spawningPositions.GetRandomPosition(nearestPlayer.SpawnRectangle);
            if (targetPos == null)
            {
                Logger.Warning("Could not find target position");
                return;
            }
            
            
            using var groupSpawn = _groupManager.NewGroup(_runnerAiPackage);
            _zombieCreator.CreateZombie(spawnPt, groupSpawn.Group);
            
            _control.SendGroupTowards(groupSpawn.Group, targetPos.Value.ToPoint(), withRandomness: false);
        }
    }
}