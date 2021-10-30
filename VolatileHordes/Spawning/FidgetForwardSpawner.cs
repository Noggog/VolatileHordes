using System.Drawing;
using System.Threading.Tasks;
using VolatileHordes.AiPackages;
using VolatileHordes.Control;
using VolatileHordes.Spawning.WanderingHordes;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public class FidgetForwardSpawner
    {
        private readonly ZombieGroupManager _groupManager;
        private readonly SpawningPositions _spawningPositions;
        private readonly WanderingHordePlacer _placer;
        private readonly ZombieControl _control;
        private readonly LimitManager _limitManager;
        private readonly FidgetForwardAIPackage _fidgetForwardAIPackage;

        public FidgetForwardSpawner(
            ZombieGroupManager groupManager,
            FidgetForwardAIPackage fidgetForwardAIPackage,
            SpawningPositions spawningPositions,
            WanderingHordePlacer placer,
            ZombieControl control,
            LimitManager limitManager)
        {
            _groupManager = groupManager;
            _spawningPositions = spawningPositions;
            _placer = placer;
            _control = control;
            _limitManager = limitManager;
            _fidgetForwardAIPackage = fidgetForwardAIPackage;
        }

        public async Task Spawn(ushort size, Point chunkPoint)
        {
            var spawnTarget = _spawningPositions.GetRandomSpawnInChunk(chunkPoint);
            if (spawnTarget == null) return;

            using var groupSpawn = _groupManager.NewGroup(_fidgetForwardAIPackage);
            
            size = _limitManager.GetAllowedLimit(size);
            
            Logger.Info("Spawning horde {0} of size {1} at {2}", groupSpawn.Group.Id, size, spawnTarget);
            
            await _placer.SpawnHorde(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.Target, size, groupSpawn.Group);
        }
    }
}