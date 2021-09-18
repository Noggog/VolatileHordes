using System.Collections.Generic;
using System.Drawing;
using UniLinq;
using UnityEngine;

namespace VolatileHordes.GameAbstractions
{
    public interface IWorld
    {
        IEnumerable<IPlayer> Players { get; }
        
        bool CanSpawnAt(Vector3 pos);
        int GetTerrainHeight(PointF pt);
        IZombie? SpawnZombie(int classId, Vector3 pos);
        void DestroyZombie(IZombie zombie);
        Chunk? GetChunkAt(PointF pt);
        Entity? GetEntity(int id);
    }

    public class WorldWrapper : IWorld
    {
        private World World => GameManager.Instance.World;

        public bool CanSpawnAt(Vector3 pos) => World.CanMobsSpawnAtPos(pos);
        
        public int GetTerrainHeight(PointF pt) => World.GetTerrainHeight((int)pt.X, (int)pt.Y);
        
        public IZombie? SpawnZombie(int classId, Vector3 pos)
        {
            if (EntityFactory.CreateEntity(classId, pos) is not EntityZombie zombieEnt)
            {
                return null;
            }

            zombieEnt.bIsChunkObserver = true;
            zombieEnt.SetSpawnerSource(EnumSpawnerSource.StaticSpawner);
            zombieEnt.IsHordeZombie = true;
            
            World.SpawnEntityInWorld(zombieEnt);
            
            return new Zombie(this, zombieEnt.entityId);
        }

        public void DestroyZombie(IZombie zombie)
        {
            World.RemoveEntity(zombie.Id, EnumRemoveEntityReason.Despawned);
        }

        public IEnumerable<IPlayer> Players => World.Players.list.Select<EntityPlayer, IPlayer>(p => new Player(p));
        
        public Chunk? GetChunkAt(PointF pt)
        {
            return World.GetChunkSync(World.toChunkXZ(pt.X.Floor()), 0, World.toChunkXZ(pt.Y.Floor())) as Chunk;
        }

        public Entity? GetEntity(int id) => World.GetEntity(id);
    }
}