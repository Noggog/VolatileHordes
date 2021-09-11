using VolatileHordes.Control;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Randomization;
using VolatileHordes.Spawning;
using VolatileHordes.Spawning.WanderingHordes;
using VolatileHordes.Zones;

namespace VolatileHordes
{
    public static class Container
    {
        public static readonly RandomSource Random = new();
        public static readonly BiomeData Biome = new(Random);
        public static readonly IWorld World = new WorldWrapper();
        public static readonly ZombieCreator ZombieCreator = new(World, Biome);
        public static readonly PlayerZoneManager PlayerZoneManager = new();
        public static readonly SpawningPositions Spawning = new(World, PlayerZoneManager, Random);
        public static readonly ZombieControl ZombieControl = new(Spawning);
        public static readonly SingleTracker SingleTracker = new(Spawning, ZombieCreator, ZombieControl);
        public static readonly WanderingHordeDirector WanderingHorde = new(SingleTracker);
    }
}