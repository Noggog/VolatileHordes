using VolatileHordes.AiPackages;
using VolatileHordes.Control;
using VolatileHordes.Core.ObservableTransforms;
using VolatileHordes.Core.Services;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Noise;
using VolatileHordes.Players;
using VolatileHordes.Probability;
using VolatileHordes.Settings.User;
using VolatileHordes.Spawning;
using VolatileHordes.Spawning.Seeker;
using VolatileHordes.Spawning.WanderingHordes;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes
{
    public static class Container
    {
        public static readonly RandomSource Random = new();
        public static readonly PointService PointService = new(Random);
        public static readonly PlayerGameEventsWrapper PlayerGameEventsWrapper = new();
        public static readonly BiomeData Biome = new(Random);
        public static readonly IWorld World = new WorldWrapper();
        public static readonly UserSettings UserSettings = UserSettings.Load();
        public static readonly GameStageCalculator GamestageCalculator = new(UserSettings.Director);
        public static readonly PlayerZoneManager PlayerZoneManager = new(World);
        public static readonly PlayerGroupProvider PlayerGroupProvider = new();
        public static readonly TimeManager Time = new(new NowProvider(), PlayerZoneManager, Random);
        public static readonly PlayerLocationUpdater PlayerLocationUpdater = new(PlayerZoneManager, Time);
        public static readonly SpawningPositions Spawning = new(World, PlayerZoneManager, Random);
        public static readonly ZombieGroupManager ZombieGroupManager = new(Time, PlayerZoneManager);
        public static readonly ZombieControl ZombieControl = new(Spawning, Time, Random);
        public static readonly NoiseManager NoiseManager = new(Time, UserSettings.Noise);
        public static readonly ILogger Logger = new LoggerWrapper();
        public static readonly TimedSignalFlowShutoff TimedSignalFlowShutoff = new(Time);
        public static readonly TemporaryAiShutoff TemporaryAiShutoff = new(TimedSignalFlowShutoff);
        public static readonly NoiseResponderControlFactory NoiseResponderControlFactory = new(Random, ZombieControl, NoiseManager, TemporaryAiShutoff, UserSettings.Control.NoiseResponder, Logger);
        public static readonly AmbientAiPackage AmbientAiPackage = new(NoiseResponderControlFactory);
        public static readonly LimitManager LimitManager = new(World, Time, ZombieGroupManager, UserSettings.Limits);
        public static readonly AmbientZombieManager Ambient = new(World, ZombieGroupManager, AmbientAiPackage, LimitManager);
        public static readonly ZombieCreator ZombieCreator = new(World, Ambient, Biome, LimitManager);
        public static readonly AmbientSpawner AmbientSpawner = new(ZombieCreator, Spawning, LimitManager);
        public static readonly PlayerSeekerControl SeekerControl = new(Time, Spawning, ZombieControl);
        public static readonly SeekerAiPackage SeekerAiPackage = new(SeekerControl);
        public static readonly ZombieGroupReachedTarget GroupReachedTarget = new(Time);
        public static readonly RunnerControl RunnerControl = new(ZombieControl, Time, Spawning, GroupReachedTarget);
        public static readonly RunnerAiPackage RunnerAiPackage = new(RunnerControl);
        public static readonly SingleRunnerSpawner SingleRunnerSpawner = new(ZombieGroupManager, RunnerAiPackage, Spawning, ZombieControl, ZombieCreator);
        public static readonly SeekerGroupCalculator SeekerCalculator = new(Random);
        public static readonly SeekerGroupSpawner SeekerGroupSpawner = new(ZombieGroupManager, SeekerCalculator, Spawning, SeekerAiPackage, ZombieCreator, ZombieControl);
        public static readonly SpawnRowPerpendicular SpawnRowPerpendicular = new(ZombieCreator, ZombieControl);
        public static readonly WanderingHordePlacer WanderingHordePlacer = new(Time, SpawnRowPerpendicular);
        public static readonly WanderingHordeCalculator WanderingHordeCalculator = new(UserSettings.WanderingHordeSettings, Random);
        public static readonly RoamControl RoamControl = new(Time, Spawning, ZombieControl);
        public static readonly FidgetRoam FidgetRoam = new(UserSettings.Control.FidgetRoam, RoamControl);
        public static readonly FidgetForwardControl FidgetForwardControl = new(UserSettings.Control.FidgetRoam, Time, Spawning, ZombieControl, PointService, Random);
        public static readonly RoamFarOccasionally RoamFarOccasionally = new(UserSettings.Control.FarRoam,RoamControl);
        public static readonly LuckyPlayerRetarget LuckyPlayerRetarget = new(Time, Random, UserSettings.Control.LuckyPlayerRetarget, Spawning, ZombieControl);
        public static readonly FidgetForwardAIPackage FidgetForwardAIPackage = new(LuckyPlayerRetarget, FidgetForwardControl);
        public static readonly RoamAiPackage RoamAiPackage = new(FidgetRoam, NoiseResponderControlFactory, RoamFarOccasionally, LuckyPlayerRetarget);
        public static readonly AiPackageMapper AiPackageMapper = new();
        public static readonly CrazyControl CrazyControl = new(ZombieControl, Time, Spawning);
        public static readonly CrazyAiPackage CrazyAiPackage = new(NoiseResponderControlFactory, CrazyControl);
        public static readonly CrazySpawner CrazySpawner = new(ZombieGroupManager, CrazyAiPackage, Spawning, ZombieControl, ZombieCreator);
        public static readonly Stats Stats = new(PlayerZoneManager, ZombieGroupManager, Ambient, LimitManager);
        public static readonly DirectorSwitch DirectorSwitch = new(UserSettings.Director);
        public static readonly FidgetForwardSpawner FidgetForwardSpawner = new(ZombieGroupManager, FidgetForwardAIPackage, Spawning, WanderingHordePlacer, ZombieControl, LimitManager);
        public static readonly WanderingHordeSpawner WanderingHordeSpawner = new(ZombieGroupManager, RoamAiPackage, Spawning, WanderingHordePlacer, ZombieControl, LimitManager);
        public static readonly BasicSpawnDirector BasicSpawnDirector = new(DirectorSwitch, Time, Random, WanderingHordeSpawner, FidgetForwardSpawner, PlayerZoneManager, GamestageCalculator);
        public static readonly AmbientDirector AmbientDirector = new(DirectorSwitch, Random, CrazyAiPackage, RunnerAiPackage, ZombieGroupManager);
    }
}