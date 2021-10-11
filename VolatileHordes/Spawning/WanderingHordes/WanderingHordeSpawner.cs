using System.Threading.Tasks;
using VolatileHordes.AiPackages;
using VolatileHordes.Control;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordeSpawner
    {
        private readonly ZombieGroupManager _groupManager;
        private readonly RoamAiPackage _roamAiPackage;
        private readonly SpawningPositions _spawningPositions;
        private readonly WanderingHordePlacer _placer;
        private readonly ZombieControl _control;

        public WanderingHordeSpawner(
            ZombieGroupManager groupManager,
            RoamAiPackage roamAiPackage,
            SpawningPositions spawningPositions,
            WanderingHordePlacer placer,
            ZombieControl control)
        {
            _groupManager = groupManager;
            _roamAiPackage = roamAiPackage;
            _spawningPositions = spawningPositions;
            _placer = placer;
            _control = control;
        }

        public async Task Spawn(int size)
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;

            using var groupSpawn = _groupManager.NewGroup(_roamAiPackage);
            
            Logger.Info("Spawning horde {0} of size {1} at {2}", groupSpawn.Group.Id, size, spawnTarget);
            
            await _placer.SpawnHorde(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.TriggerOrigin, size, groupSpawn.Group);

            _control.SendGroupTowards(groupSpawn.Group, spawnTarget.TriggerOrigin);
        }
    }
}