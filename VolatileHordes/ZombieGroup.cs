using System;
using System.Collections.Generic;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes
{
    public class ZombieGroup
    {
        public DateTime SpawnTime { get; } = DateTime.Now;
        public List<IZombie> Zombies { get; } = new();
        
        public ZombieGroup()
        {
        }
        
        public ZombieGroup(params IZombie[] zombies)
        {
            Zombies.AddRange(zombies);
        }
    }
}