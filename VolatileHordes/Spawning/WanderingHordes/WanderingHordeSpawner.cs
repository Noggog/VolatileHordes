using System.Threading.Tasks;
using VolatileHordes.AiPackages;
using VolatileHordes.Control;
using VolatileHordes.Director;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordeSpawner
    {
        private readonly GroupManager _groupManager;
        private readonly RoamAiPackage _roamAiPackage;
        private readonly WanderingHordeCalculator _hordeCalculator;
        private readonly SpawningPositions _spawningPositions;
        private readonly WanderingHordePlacer _placer;
        private readonly GameStageCalculator _gameStageCalculator;
        private readonly ZombieControl _control;

        public WanderingHordeSpawner(
            GroupManager groupManager,
            RoamAiPackage roamAiPackage,
            WanderingHordeCalculator hordeCalculator,
            SpawningPositions spawningPositions,
            WanderingHordePlacer placer,
            GameStageCalculator gameStageCalculator,
            ZombieControl control)
        {
            _groupManager = groupManager;
            _roamAiPackage = roamAiPackage;
            _hordeCalculator = hordeCalculator;
            _spawningPositions = spawningPositions;
            _placer = placer;
            _gameStageCalculator = gameStageCalculator;
            _control = control;
        }

        public async Task Spawn(int? size = null)
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;

            var gameStage = _gameStageCalculator.GetGamestage(spawnTarget.Player.Group);

            int noHorde = 0;
            size ??= _hordeCalculator.GetHordeSize(gameStage, ref noHorde);

            using var groupSpawn = _groupManager.NewGroup(_roamAiPackage);
            
            Logger.Info("Spawning horde {0} of size {1} at {2}", groupSpawn.Group.Id, size, spawnTarget);
            
            await _placer.SpawnHorde(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.TriggerOrigin, size.Value, groupSpawn.Group);

            _control.SendGroupTowards(groupSpawn.Group, spawnTarget.TriggerOrigin);
        }
    }
}