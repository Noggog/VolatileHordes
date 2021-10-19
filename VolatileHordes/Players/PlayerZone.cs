using System.Drawing;
using UnityEngine;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Players
{
    public interface IPlayerZone
    {
        RectangleF SpawnRectangle { get; }
        IPlayer Player { get; }
    }
    
    public class PlayerZone : IPlayerZone
    {
        public bool Valid { get; set; } = true;
        public PointF Mins { get; set; } = PointF.Empty;
        public PointF Maxs { get; set; } = PointF.Empty;
        public PointF MinsSpawnBlock { get; set; } = PointF.Empty;
        public PointF MaxsSpawnBlock { get; set; } = PointF.Empty;

        public PointF Center { get; set; }

        public PointF PlayerLocation => Center;
        
        public IPlayer Player { get; }

        public RectangleF SpawnRectangle => new(
            x: MinsSpawnBlock.X,
            y: MinsSpawnBlock.Y,
            width: MaxsSpawnBlock.X - MinsSpawnBlock.X,
            height: MaxsSpawnBlock.Y - MinsSpawnBlock.Y);

        static int ChunkViewDim = GamePrefs.GetInt(EnumGamePrefs.ServerMaxAllowedViewDistance);
        static Vector3 ChunkSize = new(16, 256, 16);
        static Vector3 VisibleBox = ChunkSize * ChunkViewDim;
        static Vector3 SpawnBlockBox = new(VisibleBox.x - 32, VisibleBox.y - 32, VisibleBox.z - 32);

        public PlayerZone(IPlayer player)
        {
            Player = player;
            var entityPlayer = player.TryGetEntity();
            if (entityPlayer == null)
                return;

            var pos = entityPlayer.GetPosition();
            Mins = (pos - (VisibleBox * 0.5f)).ToPoint();
            Maxs = (pos + (VisibleBox * 0.5f)).ToPoint();
            MinsSpawnBlock = (pos - (SpawnBlockBox * 0.5f)).ToPoint();
            MaxsSpawnBlock = (pos + (SpawnBlockBox * 0.5f)).ToPoint();
            Center = pos.ToPoint();
        }
    }
}