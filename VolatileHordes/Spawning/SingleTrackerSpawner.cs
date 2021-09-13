using System.Drawing;
using UnityEngine;
using VolatileHordes.Control;

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

        public void Spawn(PointF spawn, PointF target)
        {
            var zombie = _creator.CreateZombie(spawn);
            if (zombie == null) return;

            _control.SendZombieTowards(zombie, target);
        }
    }
}