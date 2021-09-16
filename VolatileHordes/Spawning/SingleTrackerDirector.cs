using VolatileHordes.ActiveDirectors;

namespace VolatileHordes.Spawning
{
    public class SingleTrackerDirector
    {
        private readonly ActiveDirector _director;
        private readonly SpawningPositions _spawningPositions;
        private readonly SingleTrackerSpawner _singleTrackerSpawner;

        public SingleTrackerDirector(
            ActiveDirector director,
            SpawningPositions spawningPositions,
            SingleTrackerSpawner singleTrackerSpawner)
        {
            _director = director;
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
                _director.TrackGroup(new ZombieGroup(zombie));
            }
        }
    }
}