﻿using System.Collections.Generic;
using System.Drawing;
using UniLinq;
using UnityEngine;
using VolatileHordes.Utility;

namespace VolatileHordes.GameAbstractions
{
    public interface IWorld
    {
        IEnumerable<IPlayer> Players { get; }
        bool TryGetPlayerEntity(int id, out EntityPlayer player);
        bool CanSpawnAt(Vector3 pos);
        int GetTerrainHeight(PointF pt);
        void SpawnZombie(EntityZombie zombie);
        void DestroyZombie(IZombie zombie);
        Chunk? GetChunkAt(PointF pt);
        Entity? GetEntity(int id);
        Vector3 GetWorldVector(PointF pt);
        EntityPlayer? GetClosestPlayer(PointF pt);
    }

    public class WorldWrapper : IWorld
    {
        private World World => GameManager.Instance.World;

        public bool TryGetPlayerEntity(int id, out EntityPlayer player)
        {
            return World.Players.dict.TryGetValue(id, out player);
        }

        public bool CanSpawnAt(Vector3 pos) => World.CanMobsSpawnAtPos(pos);
        
        public int GetTerrainHeight(PointF pt) => World.GetTerrainHeight((int)pt.X, (int)pt.Y);
        
        public void SpawnZombie(EntityZombie zombie)
        {
            World.SpawnEntityInWorld(zombie);
        }

        public void DestroyZombie(IZombie zombie)
        {
            World.RemoveEntity(zombie.Id, EnumRemoveEntityReason.Despawned);
        }

        public IEnumerable<IPlayer> Players => World.Players.list.Select<EntityPlayer, IPlayer>(p => new Player(this, p.entityId));
        
        public Chunk? GetChunkAt(PointF pt)
        {
            return World.GetChunkSync(World.toChunkXZ(pt.X.Floor()), 0, World.toChunkXZ(pt.Y.Floor())) as Chunk;
        }

        public Entity? GetEntity(int id) => World.GetEntity(id);

        public Vector3 GetWorldVector(PointF pt)
        {
            int height = GetTerrainHeight(pt);
            return pt.WithHeight(height + 1);
        }

        public EntityPlayer? GetClosestPlayer(PointF pt)
        {
            return World.GetClosestPlayer(GetWorldVector(pt), float.MaxValue, _isDead: false);
        }
    }
}