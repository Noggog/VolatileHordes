using System.Threading.Tasks;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public class AmbientSpawner
    {
        private readonly ZombieCreator _creator;
        private readonly SpawningPositions _spawningPositions;
        private readonly LimitManager _limitManager;

        public AmbientSpawner(
            ZombieCreator creator,
            SpawningPositions spawningPositions,
            LimitManager limitManager)
        {
            _creator = creator;
            _spawningPositions = spawningPositions;
            _limitManager = limitManager;
        }

        public async Task Spawn()
        {
            var zone = _spawningPositions.GetRandomPlayerZone();
            if (zone == null) return;
            
            var size = _limitManager.GetAllowedLimit(10);
            for (int i = 0; i < size; i++)
            {
                var pos = _spawningPositions.GetRandomPosition(zone.SpawnRectangle);
                if (pos == null) continue;
                await _creator.CreateZombie(pos.Value.ToPoint(), null);
            }
        }
    }
}