using System.Drawing;
using System.Threading.Tasks;
using VolatileHordes.AiPackages;
using VolatileHordes.Allocation;
using VolatileHordes.Spawning.WanderingHordes;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public class FidgetForwardSpawner
    {
        private readonly ZombieGroupManager _groupManager;
        private readonly SpawningPositions _spawningPositions;
        private readonly IChunkMeasurements _chunkMeasurements;
        private readonly WanderingHordePlacer _placer;
        private readonly LimitManager _limitManager;
        private readonly FidgetForwardAIPackage _fidgetForwardAIPackage;

        public FidgetForwardSpawner(
            ZombieGroupManager groupManager,
            FidgetForwardAIPackage fidgetForwardAIPackage,
            SpawningPositions spawningPositions,
            IChunkMeasurements chunkMeasurements,
            WanderingHordePlacer placer,
            LimitManager limitManager)
        {
            _groupManager = groupManager;
            _spawningPositions = spawningPositions;
            _chunkMeasurements = chunkMeasurements;
            _placer = placer;
            _limitManager = limitManager;
            _fidgetForwardAIPackage = fidgetForwardAIPackage;
        }

        public async Task Spawn(ushort size, Point chunkPoint)
        {
            var spawnTarget = _spawningPositions.GetRandomSpawnInChunk(chunkPoint);
            if (spawnTarget == null) return;

            using var groupSpawn = _groupManager.NewGroup(
                _chunkMeasurements.GetAllocationBucket(spawnTarget.SpawnPoint.ToPoint()),
                _fidgetForwardAIPackage);
            
            size = _limitManager.GetAllowedLimit(size);
            
            Logger.Info("Spawning horde {0} of size {1} at {2}", groupSpawn.Group.Id, size, spawnTarget);
            
            await _placer.SpawnHorde(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.Target, size, groupSpawn.Group);
        }
    }
}