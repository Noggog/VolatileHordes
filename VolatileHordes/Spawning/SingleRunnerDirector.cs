﻿using VolatileHordes.AiPackages;
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
            
            using var groupSpawn = _groupManager.NewGroup(_runnerAiPackage);
            _zombieCreator.CreateZombie(spawnTarget.SpawnPoint.ToPoint(), groupSpawn.Group);
            
            _control.SendGroupTowards(groupSpawn.Group, targetPos.Value.ToPoint(), withRandomness: false);
        }
    }
}