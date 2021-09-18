using System.Drawing;
using VolatileHordes.Control;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public class SingleTrackerSpawner
    {
        private readonly SpawnSingle _spawnSingle;
        private readonly ZombieControl _control;

        public SingleTrackerSpawner(
            SpawnSingle spawnSingle,
            ZombieControl control)
        {
            _spawnSingle = spawnSingle;
            _control = control;
        }

        public IZombie? Spawn(PointF spawn, PointF target, ZombieGroup? group)
        {
            var zombie = _spawnSingle.Spawn(spawn, group);
            if (zombie == null) return null;
            
            _control.SendZombieTowards(zombie, target);
            return zombie;
        }
    }
}