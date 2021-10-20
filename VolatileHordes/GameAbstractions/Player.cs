using System;
using System.Collections.Generic;
using System.Drawing;
using UniLinq;
using UnityEngine;
using VolatileHordes.Utility;

namespace VolatileHordes.GameAbstractions
{
    public interface IPlayer
    {
        int EntityId { get; }
        EntityPlayer? TryGetEntity();
        bool TryGetEntity(out EntityPlayer player);
        IEnumerable<Vector3> Bedrolls { get; }
        int? TryGameStage();
    }

    public class Player : IPlayer
    {
        private readonly IWorld _world;
        public int EntityId { get; }

        public IEnumerable<Vector3> Bedrolls => GetBedrolls();

        public Player(
            IWorld world,
            int id)
        {
            _world = world;
            EntityId = id;
        }

        public RectangleF SpawnRectangle => GetSpawnRectangle();
        public RectangleF VisibilityRectable => GetVisibilityRectangle();
        public PointF Location => GetLocation();

        // # Constants
        private static int ChunkViewDim = GamePrefs.GetInt(EnumGamePrefs.ServerMaxAllowedViewDistance);
        private static Vector3 ChunkSize = new(16, 256, 16);
        private static Vector3 VisibleBox = ChunkSize * ChunkViewDim;
        private static Vector3 SpawnBlockBox = new(VisibleBox.x - 32, VisibleBox.y - 32, VisibleBox.z - 32);

        private RectangleF GetSpawnRectangle()
        {
            var entity = TryGetEntity()!;
            if (entity == null) throw new Exception("entity was null");
            var pos = entity.GetPosition();
            var minsSpawnBlock = (pos - (SpawnBlockBox * 0.5f)).ToPoint();
            var maxsSpawnBlock = (pos + (SpawnBlockBox * 0.5f)).ToPoint();
            return new RectangleF(
                x: minsSpawnBlock.X,
                y: minsSpawnBlock.Y,
                width: maxsSpawnBlock.X - minsSpawnBlock.X,
                height: maxsSpawnBlock.Y - minsSpawnBlock.Y
            );
        }

        private RectangleF GetVisibilityRectangle()
        {
            var entity = TryGetEntity()!;
            if (entity == null) throw new Exception("entity was null");
            var pos = entity.GetPosition();
            var minsSpawnBlock = (pos - (VisibleBox * 0.5f)).ToPoint();
            var maxsSpawnBlock = (pos + (VisibleBox * 0.5f)).ToPoint();
            return new RectangleF(
                x: minsSpawnBlock.X,
                y: minsSpawnBlock.Y,
                width: maxsSpawnBlock.X - minsSpawnBlock.X,
                height: maxsSpawnBlock.Y - minsSpawnBlock.Y
            );
        }

        private PointF GetLocation()
        {
            var entity = TryGetEntity()!;
            if (entity == null) throw new Exception("entity was null");
            var pos = entity.GetPosition();
            return pos.ToPoint();
        }

        private IEnumerable<Vector3> GetBedrolls()
        {
            var entity = TryGetEntity();
            if (entity == null) yield break;
            for (int i = 0; i < entity.SpawnPoints.Count; ++i)
            {
                yield return entity.SpawnPoints[i].ToVector3();
            }
        }

        public EntityPlayer? TryGetEntity()
        {
            var entity = _world.GetEntity(EntityId);
            return entity as EntityPlayer;
        }

        public bool TryGetEntity(out EntityPlayer player)
        {
            player = TryGetEntity()!;
            return player != null;
        }

        public int? TryGameStage()
        {
            return TryGetEntity()?.gameStage;
        }
    }
}