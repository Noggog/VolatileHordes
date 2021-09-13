namespace VolatileHordes.Spawning
{
    public class SingleTrackerDirector
    {
        private readonly SpawningPositions _spawningPositions;
        private readonly SingleTrackerSpawner _singleTrackerSpawner;

        public SingleTrackerDirector(
            SpawningPositions spawningPositions,
            SingleTrackerSpawner singleTrackerSpawner)
        {
            _spawningPositions = spawningPositions;
            _singleTrackerSpawner = singleTrackerSpawner;
        }

        public void SpawnSingle()
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;
            
            _singleTrackerSpawner.Spawn(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.TriggerOrigin);
        }
    }
}