using System.Drawing;
using UnityEngine;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public class ZombieCreator
    {
        private readonly IWorld _world;
        private readonly AmbientZombieManager _ambientZombieManager;
        private readonly BiomeData _biomeData;
        private static readonly int MaxAliveZombies = GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies);
        private static readonly int MaxSpawnedZombies = (int)(MaxAliveZombies * 0.5);

        public int CurrentlyActiveZombies => GameStats.GetInt(EnumGameStats.EnemyCount);

        public ZombieCreator(
            IWorld world,
            AmbientZombieManager ambientZombieManager,
            BiomeData biomeData)
        {
            _world = world;
            _ambientZombieManager = ambientZombieManager;
            _biomeData = biomeData;
        }

        public bool CanSpawnZombie()
        {
            if (CurrentlyActiveZombies + 1 >= MaxSpawnedZombies)
                return false;
            return true;
        }

        public void PrintZombieStats()
        {
            Logger.Info("Currently {0} zombies. {1}% of total. {2}% of allowed", CurrentlyActiveZombies, (100.0f * CurrentlyActiveZombies / MaxAliveZombies), (100.0f * CurrentlyActiveZombies / MaxSpawnedZombies));
        }
        
        public bool IsSpawnProtected(Vector3 pos)
        {
            var players = _world.Players;

            foreach (var ply in players)
            {
                foreach (var bedrollPos in ply.Bedrolls)
                {
                    var dist = Vector3.Distance(pos, bedrollPos);
                    if (dist <= 50)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanZombieSpawnAt(Vector3 pos)
        {
            if (!_world.CanSpawnAt(pos))
                return false;

            if (IsSpawnProtected(pos))
                return false;

            return true;
        }
        
        public IZombie? CreateZombie(PointF spawnLocation, ZombieGroup? group)
        {
            if (!CanSpawnZombie())
            {
                Logger.Warning("Too many zombies already.");
                return null;
            }

            Chunk? chunk = _world.GetChunkAt(spawnLocation);
            if (chunk == null)
            {
                Logger.Debug("Chunk not loaded at {0}", spawnLocation);
                return null;
            }

            var worldSpawn = _world.GetWorldVector(spawnLocation);
    
            if (!CanZombieSpawnAt(worldSpawn))
            {
                Logger.Debug("Unable to spawn zombie at {0}, CanMobsSpawnAtPos failed", spawnLocation);
                return null;
            }
    
            var classId = _biomeData.GetZombieClass(
                chunk, 
                (int)spawnLocation.X,
                (int)spawnLocation.Y);
            if (classId == -1)
            {
                int lastClassId = -1;
                classId = EntityGroups.GetRandomFromGroup("ZombiesAll", ref lastClassId);
                Logger.Debug("Used fallback for zombie class!");
            }
            
            if (EntityFactory.CreateEntity(classId, worldSpawn) is not EntityZombie zombieEnt)
            {
                Logger.Error("Unable to create zombie entity!, Entity Id: {0}, Pos: {1}", classId, worldSpawn);
                return null;
            }

            zombieEnt.bIsChunkObserver = true;
            zombieEnt.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);
            zombieEnt.IsHordeZombie = true;
            
            var zombie = new Zombie(_world, zombieEnt.entityId);
            
            group?.Add(zombie);
            _ambientZombieManager.MarkTracked(zombie);
    
            _world.SpawnZombie(zombieEnt);
            Logger.Debug("Spawned zombie {0} at {1} into group {2}", zombie, worldSpawn, group?.ToString() ?? "None");
    
            return zombie;
        }
    }
}