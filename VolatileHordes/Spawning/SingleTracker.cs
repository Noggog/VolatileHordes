using VolatileHordes.Control;

namespace VolatileHordes.Spawning
{
    public class SingleTracker
    {
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieCreator _creator;
        private readonly ZombieControl _control;

        public SingleTracker(
            SpawningPositions spawningPositions,
            ZombieCreator creator,
            ZombieControl control)
        {
            _spawningPositions = spawningPositions;
            _creator = creator;
            _control = control;
        }

        public void SpawnSingle()
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;
            
            var zombie = _creator.CreateZombie(spawnTarget.SpawnPoint, spawnTarget.TriggerOrigin);
            if (zombie == null) return;
            
            _control.SendZombieTowards(zombie, spawnTarget.TriggerOrigin);
        }
    }
}