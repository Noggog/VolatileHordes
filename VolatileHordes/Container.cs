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
        public static readonly PlayerZoneManager PlayerZoneManager = new();
        public static readonly SpawningPositions Spawning = new(World, PlayerZoneManager, Random);
        public static readonly ZombieCreator ZombieCreator = new(World, Spawning, Biome);
        public static readonly ZombieControl ZombieControl = new(Spawning);
        public static readonly SingleTrackerSpawner SingleTrackerSpawner = new(ZombieCreator, ZombieControl);
        public static readonly SingleTrackerDirector SingleTrackerDirector = new(Spawning, SingleTrackerSpawner);
        public static readonly SpawnRowPerpendicular SpawnRowPerpendicular = new(SingleTrackerSpawner);
        public static readonly WanderingHordeSpawner WanderingHordeSpawner = new(SpawnRowPerpendicular);
        public static readonly WanderingHordeCalculator WanderingHordeCalculator = new(Random);
        public static readonly GamestageCalculator GamestageCalculator = new(PlayerZoneManager);
        public static readonly Settings Settings = Settings.Load();
        public static readonly WanderingHordeDirector WanderingHordeDirector = new(Settings.WanderingHordeSettings, GamestageCalculator, WanderingHordeCalculator, Spawning, WanderingHordeSpawner);
    }
}