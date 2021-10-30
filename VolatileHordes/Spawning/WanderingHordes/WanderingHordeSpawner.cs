using System.Drawing;
using System.Threading.Tasks;
using VolatileHordes.AiPackages;
using VolatileHordes.Allocation;
using VolatileHordes.Control;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordeSpawner : IHordeSpawner
    {
        private readonly ZombieGroupManager _groupManager;
        private readonly RoamAiPackage _roamAiPackage;
        private readonly SpawningPositions _spawningPositions;
        private readonly WanderingHordePlacer _placer;
        private readonly ZombieControl _control;
        private readonly IChunkMeasurements _chunkMeasurements;
        private readonly LimitManager _limitManager;

        public WanderingHordeSpawner(
            ZombieGroupManager groupManager,
            RoamAiPackage roamAiPackage,
            SpawningPositions spawningPositions,
            WanderingHordePlacer placer,
            ZombieControl control,
            IChunkMeasurements chunkMeasurements,
            LimitManager limitManager)
        {
            _groupManager = groupManager;
            _roamAiPackage = roamAiPackage;
            _spawningPositions = spawningPositions;
            _placer = placer;
            _control = control;
            _chunkMeasurements = chunkMeasurements;
            _limitManager = limitManager;
        }
        
        public async Task Spawn(ushort size)
        {
            var zone = _spawningPositions.GetRandomPlayerZone();
            if (zone == null)
            {
                Logger.Warning("Could not spawn horde because no player was found.");
                return;
            }
            
            await Spawn(size, _chunkMeasurements.GetAllocationBucket(zone.Center));
        }

        public async Task Spawn(ushort size, Point chunkPoint)
        {
            var spawnTarget = _spawningPositions.GetRandomSpawnInChunk(chunkPoint);
            if (spawnTarget == null) return;

            using var groupSpawn = _groupManager.NewGroup(
                _chunkMeasurements.GetAllocationBucket(spawnTarget.SpawnPoint.ToPoint()),
                _roamAiPackage);
            
            Logger.Info("Spawning horde {0} of size {1} at {2}", groupSpawn.Group.Id, size, spawnTarget);
            
            size = _limitManager.GetAllowedLimit(size);
            
            await _placer.SpawnHorde(spawnTarget.SpawnPoint.ToPoint(), spawnTarget.Target, size, groupSpawn.Group);

            _control.SendGroupTowards(groupSpawn.Group, spawnTarget.Target);
        }
    }
}