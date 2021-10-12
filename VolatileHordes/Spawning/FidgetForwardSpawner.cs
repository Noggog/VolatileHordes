using System.Threading.Tasks;
using VolatileHordes.AiPackages;
using VolatileHordes.Control;
using VolatileHordes.Director;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class FidgetForwardSpawner
    {
        private readonly ZombieGroupManager _groupManager;
        private readonly SpawningPositions _spawningPositions;
        private readonly WanderingHordePlacer _placer;
        private readonly ZombieControl _control;
        private readonly LimitManager _limitManager;
        private readonly FidgetForwardAIPackage fidgetForwardAIPackage;

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
            this.fidgetForwardAIPackage = fidgetForwardAIPackage;
        }

        public async Task Spawn(ushort size)
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;

            using var groupSpawn = _groupManager.NewGroup(fidgetForwardAIPackage);
            
            size = _limitManager.GetAllowedLimit(size);
            
            Logger.Info("Spawning horde {0} of size {1} at {2}", groupSpawn.Group.Id, size, spawnTarget);
            
            await _placer.SpawnHorde(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.TriggerOrigin, size, groupSpawn.Group);

            _control.SendGroupTowards(groupSpawn.Group, spawnTarget.TriggerOrigin);
        }
    }
}