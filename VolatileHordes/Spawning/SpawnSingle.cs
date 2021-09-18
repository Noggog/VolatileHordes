using System.Drawing;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public class SpawnSingle
    {
        private readonly ZombieCreator _creator;

        public SpawnSingle(
            ZombieCreator creator)
        {
            _creator = creator;
        }

        public IZombie? Spawn(PointF spawn, ZombieGroup? group)
        {
            var zombie = _creator.CreateZombie(spawn);
            if (zombie == null) return null;

            group?.Zombies.Add(zombie);
            return zombie;
        }
    }
}