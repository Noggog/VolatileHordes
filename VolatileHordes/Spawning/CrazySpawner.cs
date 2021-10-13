using System.Threading.Tasks;
using VolatileHordes.AiPackages;
using VolatileHordes.Control;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public class CrazySpawner
    {
        private readonly ZombieGroupManager _groupManager;
        private readonly CrazyAiPackage _crazyAiPackage;
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieControl _control;
        private readonly ZombieCreator _zombieCreator;

        public CrazySpawner(
            ZombieGroupManager groupManager,
            CrazyAiPackage crazyAiPackage,
            SpawningPositions spawningPositions,
            ZombieControl control,
            ZombieCreator zombieCreator)
        {
            _groupManager = groupManager;
            _crazyAiPackage = crazyAiPackage;
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
            
            using var groupSpawn = _groupManager.NewGroup(_crazyAiPackage);
            await _zombieCreator.CreateZombie(spawnTarget.SpawnPoint.ToPoint(), groupSpawn.Group);
            
            _control.SendGroupTowards(groupSpawn.Group, targetPos.Value.ToPoint());
        }
    }
}