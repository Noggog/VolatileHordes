using UnityEngine;
using VolatileHordes.Randomization;
using VolatileHordes.Zones;

namespace VolatileHordes.Spawning
{
    public class ZombieCreator
    {
        private readonly RandomSource _randomSource;
        public static readonly ZombieCreator Instance = new(RandomSource.Instance);
        
        private static readonly int MaxAliveZombies = GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies);
        private static readonly int MaxSpawnedZombies = (int)(MaxAliveZombies * 0.5);

        public ZombieCreator(RandomSource randomSource)
        {
            _randomSource = randomSource;
        }

        bool CanSpawnActiveZombie()
        {
            int alive = GameStats.GetInt(EnumGameStats.EnemyCount);
            if (alive + 1 >= MaxSpawnedZombies)
                return false;
            return true;
        }
        
        public static bool IsSpawnProtected(Vector3 pos)
        {
            var world = GameManager.Instance.World;
            var players = world.Players.list;

            foreach (var ply in players)
            {
                for (int i = 0; i < ply.SpawnPoints.Count; ++i)
                {
                    var spawnPos = ply.SpawnPoints[i].ToVector3();
                    var dist = Vector3.Distance(pos, spawnPos);
                    if (dist <= 50)
                        return true;
                }
            }
            return false;
        }

        public static bool CanZombieSpawnAt(Vector3 pos)
        {
            var world = GameManager.Instance.World;

            if (!world.CanMobsSpawnAtPos(pos))
                return false;

            if (IsSpawnProtected(pos))
                return false;

            return true;
        }
        
        public bool CreateZombie(PlayerZone zone, Vector3? target)
        {
            if (!CanSpawnActiveZombie())
            {
                Logger.Warning("Too many zombies already.");
                return false;
            }
    
            var world = GameManager.Instance.World;
            
            var randomLocation = zone.GetRandomZonePos(_randomSource);
            if (randomLocation == null)
            {
                Logger.Warning("Could not find random location.");
                return false;
            }
            
            Chunk? chunk = world.GetChunkSync(World.toChunkXZ(Mathf.FloorToInt(randomLocation.Value.x)), 0,
                World.toChunkXZ(Mathf.FloorToInt(randomLocation.Value.z))) as Chunk;
            if (chunk == null)
            {
                Logger.Debug("Chunk not loaded at {0} {1}", randomLocation, randomLocation.Value.z);
                return false;
            }
    
            int height = world.GetTerrainHeight(Mathf.FloorToInt(randomLocation.Value.x), Mathf.FloorToInt(randomLocation.Value.z));
    
            Vector3 spawnPos = new Vector3(randomLocation.Value.x, height + 1.0f, randomLocation.Value.z);
            if (!CanZombieSpawnAt(spawnPos))
            {
                Logger.Debug("Unable to spawn zombie at {0}, CanMobsSpawnAtPos failed", spawnPos);
                return false;
            }
    
            var classId = BiomeData.Instance.GetZombieClass(world, chunk, (int)spawnPos.x, (int)spawnPos.z, RandomSource.Instance);
            if (classId == -1)
            {
                int lastClassId = -1;
                classId = EntityGroups.GetRandomFromGroup("ZombiesAll", ref lastClassId);
                Logger.Debug("Used fallback for zombie class!");
            }
    
            if (EntityFactory.CreateEntity(classId, spawnPos) is not EntityZombie zombieEnt)
            {
                Logger.Error("Unable to create zombie entity!, Entity Id: {0}, Pos: {1}", classId, spawnPos);
                return false;
            }
    
            zombieEnt.bIsChunkObserver = true;
    
            // TODO: Figure out a better way to make them walk towards something.
            // Send zombie towards a random position in the zone.
            if (target != null)
            {
                Logger.Debug("Sending zombie towards {0}", target.Value);
                zombieEnt.SetInvestigatePosition(target.Value, 6000, false);
            }
    
            zombieEnt.IsHordeZombie = false;
            zombieEnt.IsBloodMoon = false;
    
            zombieEnt.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);
    
            world.SpawnEntityInWorld(zombieEnt);
    
            Logger.Debug("Spawned zombie {0} at {1}", zombieEnt, spawnPos);
    
            return true;
        }
    }
}