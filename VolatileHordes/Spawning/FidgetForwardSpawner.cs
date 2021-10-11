using System;
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
        private readonly WanderingHordeCalculator _hordeCalculator;
        private readonly SpawningPositions _spawningPositions;
        private readonly WanderingHordePlacer wanderingHordePlacer;
        private readonly GameStageCalculator _gameStageCalculator;
        private readonly ZombieControl _control;
        private readonly FidgetForwardAIPackage fidgetForwardAIPackage;
        private readonly TimeManager timeManager;

        public FidgetForwardSpawner(
            ZombieGroupManager groupManager,
            FidgetForwardAIPackage fidgetForwardAIPackage,
            WanderingHordeCalculator hordeCalculator,
            SpawningPositions spawningPositions,
            WanderingHordePlacer wanderingHordePlacer,
            GameStageCalculator gameStageCalculator,
            ZombieControl control,
            TimeManager timeManager)
        {
            _groupManager = groupManager;
            _hordeCalculator = hordeCalculator;
            _spawningPositions = spawningPositions;
            this.wanderingHordePlacer = wanderingHordePlacer;
            _gameStageCalculator = gameStageCalculator;
            _control = control;
            this.fidgetForwardAIPackage = fidgetForwardAIPackage;
            this.timeManager = timeManager;
        }

        public async Task Spawn(int? size = null)
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;

            var gameStage = _gameStageCalculator.GetGamestage(spawnTarget.Player.Group);

            int noHorde = 0;
            size ??= _hordeCalculator.GetHordeSize(gameStage, ref noHorde);

            using var groupSpawn = _groupManager.NewGroup(fidgetForwardAIPackage);

            Logger.Info("Spawning horde {0} of size {1} at {2}", groupSpawn.Group.Id, size, spawnTarget);

            await wanderingHordePlacer.SpawnHorde(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.TriggerOrigin, size.Value, groupSpawn.Group);

            _control.SendGroupTowards(groupSpawn.Group, spawnTarget.TriggerOrigin);

            // Duct-tape solution so that zombies don't overpopulate
            timeManager.Timer(TimeSpan.FromMinutes(10))
                .Subscribe(x => groupSpawn.Group.Destroy());
        }
    }
}