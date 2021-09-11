using VolatileHordes.Control;

namespace VolatileHordes.Spawning
{
    public class SingleTracker
    {
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieControl _control;

        public static readonly SingleTracker Instance = new(
            SpawningPositions.Instance,
            ZombieControl.Instance);

        public SingleTracker(
            SpawningPositions spawningPositions,
            ZombieControl control)
        {
            _spawningPositions = spawningPositions;
            _control = control;
        }

        public void SpawnSingle()
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;
            
            var zombie = ZombieCreator.Instance.CreateZombie(spawnTarget.SpawnPoint, spawnTarget.TriggerOrigin);
            if (zombie == null) return;
            
            _control.SendZombieTowards(zombie, spawnTarget.TriggerOrigin);
        }
    }
}