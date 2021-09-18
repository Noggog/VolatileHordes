using System;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public struct ZombieGroupSpawn : IDisposable
    {
        public readonly ZombieGroup Group;

        public ZombieGroupSpawn(ZombieGroup group)
        {
            Group = group;
        }

        public void Dispose()
        {
            Group.AiPackage?.ApplyTo(Group);
        }
    }
}