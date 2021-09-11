using System;
using System.Drawing;
using UnityEngine;
using VolatileHordes.Randomization;
using VolatileHordes.Zones;

namespace VolatileHordes.Spawning
{
    public class ZombieCreator
    {
        public static readonly ZombieCreator Instance = new();
        
        private static readonly int MaxAliveZombies = GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies);
        private static readonly int MaxSpawnedZombies = (int)(MaxAliveZombies * 0.5);
        private readonly World _world = GameManager.Instance.World;

        public int CurrentlyActiveZombies => GameStats.GetInt(EnumGameStats.EnemyCount);

        public ZombieCreator()
        {
        }

        public bool CanSpawnZombie()
        {
            if (CurrentlyActiveZombies + 1 >= MaxSpawnedZombies)
                return false;
            return true;
        }

        public void PrintZombieStats()
        {
#if DEBUG
            Logger.Debug("Currently {0} zombies. {1}% of total. {2}% of allowed", CurrentlyActiveZombies, (100.0f * CurrentlyActiveZombies / MaxAliveZombies), (100.0f * CurrentlyActiveZombies / MaxSpawnedZombies));
#endif
        }
        
        public bool IsSpawnProtected(Vector3 pos)
        {
            var players = _world.Players.list;

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

        public bool CanZombieSpawnAt(Vector3 pos)
        {
            var world = GameManager.Instance.World;

            if (!world.CanMobsSpawnAtPos(pos))
                return false;

            if (IsSpawnProtected(pos))
                return false;

            return true;
        }
        
        public bool CreateZombie(Vector3 spawnLocation, PointF? target)
        {
            if (!CanSpawnZombie())
            {
                Logger.Warning("Too many zombies already.");
                return false;
            }
            
            var world = GameManager.Instance.World;
            Chunk? chunk = world.GetChunkSync(World.toChunkXZ(spawnLocation.x.Floor()), 0,
                World.toChunkXZ(spawnLocation.z.Floor())) as Chunk;
            if (chunk == null)
            {
                Logger.Debug("Chunk not loaded at {0} {1}", spawnLocation.x, spawnLocation.z);
                return false;
            }
    
            if (!CanZombieSpawnAt(spawnLocation))
            {
                Logger.Debug("Unable to spawn zombie at {0}, CanMobsSpawnAtPos failed", spawnLocation);
                return false;
            }
    
            var classId = BiomeData.Instance.GetZombieClass(world, chunk, (int)spawnLocation.x, (int)spawnLocation.z, RandomSource.Instance);
            if (classId == -1)
            {
                int lastClassId = -1;
                classId = EntityGroups.GetRandomFromGroup("ZombiesAll", ref lastClassId);
                Logger.Debug("Used fallback for zombie class!");
            }
    
            if (EntityFactory.CreateEntity(classId, spawnLocation) is not EntityZombie zombieEnt)
            {
                Logger.Error("Unable to create zombie entity!, Entity Id: {0}, Pos: {1}", classId, spawnLocation);
                return false;
            }
    
            Logger.Debug("Spawning zombie {0} at {1}", zombieEnt, spawnLocation);
    
            zombieEnt.bIsChunkObserver = true;
            zombieEnt.IsHordeZombie = true;
            zombieEnt.IsBloodMoon = false;
    
            // TODO: Figure out a better way to make them walk towards something.
            // Send zombie towards a random position in the zone.
            if (target != null)
            {
                var worldTarget = SpawningPositions.Instance.GetWorldVector(target.Value);
                Logger.Debug("Sending zombie towards {0}", worldTarget);
                zombieEnt.SetInvestigatePosition(worldTarget, 6000, false);
            }
    
            zombieEnt.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);
            world.SpawnEntityInWorld(zombieEnt);
    
            return true;
        }
    }
}