using System.Drawing;
using VolatileHordes.Control;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Spawning
{
    public class SingleTrackerSpawner
    {
        private readonly ZombieCreator _creator;
        private readonly ZombieControl _control;

        public SingleTrackerSpawner(
            ZombieCreator creator,
            ZombieControl control)
        {
            _creator = creator;
            _control = control;
        }

        public IZombie? Spawn(PointF spawn, PointF target, ZombieGroup? group)
        {
            var zombie = _creator.CreateZombie(spawn);
            if (zombie == null) return null;

            group?.Zombies.Add(zombie);
            
            _control.SendZombieTowards(zombie, target);
            return zombie;
        }
    }
}