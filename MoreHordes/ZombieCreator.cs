using UnityEngine;

namespace MoreHordes
{
    public class ZombieCreator
    {
        public static readonly ZombieCreator Instance = new();
        
        private static readonly int MaxAliveZombies = GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies);
        private static readonly int MaxSpawnedZombies = (int)(MaxAliveZombies * 0.5);

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
        
        public bool CreateZombie(PlayerZone zone)
        {
            if (!CanSpawnActiveZombie())
            {
                return false;
            }
    
            var world = GameManager.Instance.World;
            
            var randomLocation = zone.GetRandomZonePos();
            if (randomLocation == null)
            {
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
            // var targetPos = zone.GetRandomZonePos();
            // if (targetPos == null)
            // {
            //     Logger.Error("Had to send zombie to center zone.");
            //     targetPos = zone.center;
            // }
            //
            // zombieEnt.SetInvestigatePosition(targetPos.Value, 6000, false);
    
            zombieEnt.IsHordeZombie = false;
            zombieEnt.IsBloodMoon = true; //_state.IsBloodMoon;
    
            zombieEnt.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);
    
            world.SpawnEntityInWorld(zombieEnt);
    
            Logger.Debug("Spawned zombie {0} at {1}", zombieEnt, spawnPos);
    
            return true;
        }
    }
}