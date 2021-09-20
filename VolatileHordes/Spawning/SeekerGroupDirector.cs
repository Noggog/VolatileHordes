using VolatileHordes.AiPackages;
using VolatileHordes.Control;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public class SeekerGroupDirector
    {
        private readonly GroupManager _groupManager;
        private readonly SpawningPositions _spawningPositions;
        private readonly SeekerAiPackage _seekerAiPackage;
        private readonly ZombieCreator _zombieCreator;
        private readonly ZombieControl _control;

        public SeekerGroupDirector(
            GroupManager groupManager,
            SpawningPositions spawningPositions,
            SeekerAiPackage seekerAiPackage,
            ZombieCreator zombieCreator,
            ZombieControl control)
        {
            _groupManager = groupManager;
            _spawningPositions = spawningPositions;
            _seekerAiPackage = seekerAiPackage;
            _zombieCreator = zombieCreator;
            _control = control;
        }

        public void Spawn()
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;
            
            using var groupSpawn = _groupManager.NewGroup(_seekerAiPackage);
            _zombieCreator.CreateZombie(spawnTarget.SpawnPoint.ToPoint(), groupSpawn.Group);
            _control.SendGroupTowards(groupSpawn.Group, spawnTarget.TriggerOrigin);
        }
    }
}