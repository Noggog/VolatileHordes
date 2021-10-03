namespace VolatileHordes.Spawning
{
    public class AmbientSpawner
    {
        private readonly ZombieCreator _creator;
        private readonly SpawningPositions _spawningPositions;

        public AmbientSpawner(
            ZombieCreator creator,
            SpawningPositions spawningPositions)
        {
            _creator = creator;
            _spawningPositions = spawningPositions;
        }

        public void Spawn()
        {
            var zone = _spawningPositions.GetRandomZone();
            if (zone == null) return;
            for (int i = 0; i < 10; i++)
            {
                var pos = _spawningPositions.GetRandomPosition(zone.SpawnRectangle);
                if (pos == null) continue;
                _creator.CreateZombie(pos.Value.ToPoint(), null);
            }
        }
    }
}