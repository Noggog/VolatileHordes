namespace VolatileHordes.Spawning
{
    public class SingleTrackerDirector
    {
        private readonly GroupManager _groupManager;
        private readonly SpawningPositions _spawningPositions;
        private readonly SingleTrackerSpawner _singleTrackerSpawner;

        public SingleTrackerDirector(
            GroupManager groupManager,
            SpawningPositions spawningPositions,
            SingleTrackerSpawner singleTrackerSpawner)
        {
            _groupManager = groupManager;
            _spawningPositions = spawningPositions;
            _singleTrackerSpawner = singleTrackerSpawner;
        }

        public void SpawnSingle()
        {
            var spawnTarget = _spawningPositions.GetRandomTarget();
            if (spawnTarget == null) return;
            
            var zombie = _singleTrackerSpawner.Spawn(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.TriggerOrigin, null);
            if (zombie != null)
            {
                _groupManager.NewGroup()
                    .Zombies.Add(zombie);
            }
        }
    }
}