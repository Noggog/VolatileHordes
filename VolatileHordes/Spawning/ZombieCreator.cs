using System;
using System.Drawing;
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

        public int CurrentlyActiveZombies => GameStats.GetInt(EnumGameStats.EnemyCount);

        public ZombieCreator(RandomSource randomSource)
        {
            _randomSource = randomSource;
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
        
        public bool CreateZombie(PlayerZone zone, PointF? target)
        {
            if (!CanSpawnZombie())
            {
                Logger.Warning("Too many zombies already.");
                return false;
            }
    
            var world = GameManager.Instance.World;
            
            var randomLocation = SpawningPositions.Instance.GetRandomZonePos(zone);
            if (randomLocation == null)
            {
                Logger.Warning("Could not find random location.");
                return false;
            }
            
            Chunk? chunk = world.GetChunkSync(World.toChunkXZ(randomLocation.Value.X.Floor()), 0,
                World.toChunkXZ(randomLocation.Value.Y.Floor())) as Chunk;
            if (chunk == null)
            {
                Logger.Debug("Chunk not loaded at {0} {1}", randomLocation, randomLocation.Value.Y);
                return false;
            }
    
            int height = world.GetTerrainHeight(randomLocation.Value.X.Floor(), randomLocation.Value.Y.Floor());
    
            Vector3 spawnPos = new Vector3(randomLocation.Value.X, height + 1.0f, randomLocation.Value.Y);
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
                var spawnTarget = SpawningPositions.Instance.GetWorldVector(target.Value);
                Logger.Debug("Sending zombie towards {0}", spawnTarget);
                zombieEnt.SetInvestigatePosition(spawnTarget, 6000, false);
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