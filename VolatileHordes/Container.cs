using VolatileHordes.AiPackages;
using VolatileHordes.Control;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Randomization;
using VolatileHordes.Settings.User;
using VolatileHordes.Spawning;
using VolatileHordes.Spawning.WanderingHordes;
using VolatileHordes.Tracking;
using VolatileHordes.Zones;

namespace VolatileHordes
{
    public static class Container
    {
        public static readonly RandomSource Random = new();
        public static readonly BiomeData Biome = new(Random);
        public static readonly IWorld World = new WorldWrapper();
        public static readonly TimeManager Time = new(Random);
        public static readonly PlayerZoneManager PlayerZoneManager = new(Time);
        public static readonly SpawningPositions Spawning = new(World, PlayerZoneManager, Random);
        public static readonly ZombieCreator ZombieCreator = new(World, Spawning, Biome);
        public static readonly ZombieControl ZombieControl = new(Spawning);
        public static readonly SpawnSingle SpawnSingle = new(ZombieCreator);
        public static readonly SingleTrackerSpawner SingleTrackerSpawner = new(SpawnSingle, ZombieControl);
        public static readonly GroupManager GroupManager = new(Time, PlayerZoneManager);
        public static readonly SingleTrackerDirector SingleTrackerDirector = new(GroupManager, Spawning, SingleTrackerSpawner);
        public static readonly SpawnRowPerpendicular SpawnRowPerpendicular = new(SingleTrackerSpawner);
        public static readonly WanderingHordeSpawner WanderingHordeSpawner = new(Time, SpawnRowPerpendicular);
        public static readonly WanderingHordeCalculator WanderingHordeCalculator = new(Random);
        public static readonly GamestageCalculator GamestageCalculator = new(PlayerZoneManager);
        public static readonly UserSettings UserSettings = UserSettings.Load();
        public static readonly RoamControl RoamControl = new(Time, Spawning, ZombieControl);
        public static readonly FidgetRoam FidgetRoam = new(UserSettings.Control.FidgetRoam, RoamControl);
        public static readonly RoamFarOccasionally RoamFarOccasionally = new(UserSettings.Control.FarRoam,RoamControl);
        public static readonly RoamAiPackage RoamAiPackage = new(FidgetRoam, RoamFarOccasionally);
        public static readonly WanderingHordeDirector WanderingHordeDirector = new(GroupManager, UserSettings.WanderingHordeSettings, RoamAiPackage, GamestageCalculator, WanderingHordeCalculator, Spawning, WanderingHordeSpawner, ZombieControl);
        public static readonly AiPackageMapper AiPackageMapper = new();
    }
}