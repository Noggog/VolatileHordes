using System.Threading.Tasks;
using VolatileHordes.AiPackages;
using VolatileHordes.Control;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordeDirector
    {
        private readonly GroupManager _groupManager;
        private readonly WanderingHordeSettings _settings;
        private readonly RoamAiPackage _roamAiPackage;
        private readonly GamestageCalculator _gamestageCalculator;
        private readonly WanderingHordeCalculator _hordeCalculator;
        private readonly SpawningPositions _spawningPositions;
        private readonly WanderingHordeSpawner _spawner;
        private readonly ZombieControl _control;

        public WanderingHordeDirector(
            GroupManager groupManager,
            WanderingHordeSettings settings,
            RoamAiPackage roamAiPackage,
            GamestageCalculator gamestageCalculator,
            WanderingHordeCalculator hordeCalculator,
            SpawningPositions spawningPositions,
            WanderingHordeSpawner spawner,
            ZombieControl control)
        {
            _groupManager = groupManager;
            _settings = settings;
            _roamAiPackage = roamAiPackage;
            _gamestageCalculator = gamestageCalculator;
            _hordeCalculator = hordeCalculator;
            _spawningPositions = spawningPositions;
            _spawner = spawner;
            _control = control;
        }

        public async Task Spawn(int? size = null)
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;

            int noHorde = 0;
            size ??= _hordeCalculator.GetHordeSize(_settings, ref noHorde,
                _gamestageCalculator.GetEffectiveGamestage());

            using var groupSpawn = _groupManager.NewGroup(_roamAiPackage);
            
            Logger.Info("Spawning horde {0} of size {1} at {2}", groupSpawn.Group, size, spawnTarget);
            
            await _spawner.SpawnHorde(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.TriggerOrigin, size.Value, groupSpawn.Group);

            _control.SendGroupTowards(groupSpawn.Group, spawnTarget.TriggerOrigin);
        }
    }
}