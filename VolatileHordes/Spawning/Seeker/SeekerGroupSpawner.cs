using System.Threading.Tasks;
using VolatileHordes.AiPackages;
using VolatileHordes.Allocation;
using VolatileHordes.Control;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning.Seeker
{
    public class SeekerGroupSpawner
    {
        private readonly ZombieGroupManager _groupManager;
        private readonly SeekerGroupCalculator _calculator;
        private readonly SpawningPositions _spawningPositions;
        private readonly IChunkMeasurements _chunkMeasurements;
        private readonly SeekerAiPackage _seekerAiPackage;
        private readonly ZombieCreator _zombieCreator;
        private readonly ZombieControl _control;

        public SeekerGroupSpawner(
            ZombieGroupManager groupManager,
            SeekerGroupCalculator calculator,
            SpawningPositions spawningPositions,
            IChunkMeasurements chunkMeasurements,
            SeekerAiPackage seekerAiPackage,
            ZombieCreator zombieCreator,
            ZombieControl control)
        {
            _groupManager = groupManager;
            _calculator = calculator;
            _spawningPositions = spawningPositions;
            _chunkMeasurements = chunkMeasurements;
            _seekerAiPackage = seekerAiPackage;
            _zombieCreator = zombieCreator;
            _control = control;
        }

        public async Task Spawn()
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null)
            {
                Logger.Warning("Could not find location to spawn seeker group");
                return;
            }
            
            var size = _calculator.GetSeekerGroupSize(spawnTarget.Player);
            
            using var groupSpawn = _groupManager.NewGroup(
                _chunkMeasurements.GetAllocationBucket(spawnTarget.SpawnPoint.ToPoint()),
                _seekerAiPackage);
            for (int i = 0; i < size; i++)
            {
                await _zombieCreator.CreateZombie(spawnTarget.SpawnPoint.ToPoint(), groupSpawn.Group);
            }
            _control.SendGroupTowards(groupSpawn.Group, spawnTarget.Player.Center);
        }
    }
}