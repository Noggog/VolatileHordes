namespace VolatileHordes.Spawning
{
    public class SingleTracker
    {
        private readonly SpawningPositions _spawningPositions;
        public static readonly SingleTracker Instance = new(SpawningPositions.Instance);

        public SingleTracker(
            SpawningPositions spawningPositions)
        {
            _spawningPositions = spawningPositions;
        }

        public void SpawnSingle()
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null)
            { 
                Logger.Info("No player to spawn next to");
                return;
            }
            ZombieCreator.Instance.CreateZombie(spawnTarget.SpawnPoint, spawnTarget.TriggerOrigin);
        }
    }
}