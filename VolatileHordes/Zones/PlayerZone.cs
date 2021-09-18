using System.Drawing;

namespace VolatileHordes.Zones
{
    public interface ISpawnTarget
    {
        RectangleF SpawnRectangle { get; }
        int EntityId { get; }
    }
    
    public class PlayerZone : ISpawnTarget
    {
        public bool Valid { get; set; } = true;
        public PointF Mins { get; set; } = PointF.Empty;
        public PointF Maxs { get; set; } = PointF.Empty;
        public PointF MinsSpawnBlock { get; set; } = PointF.Empty;
        public PointF MaxsSpawnBlock { get; set; } = PointF.Empty;
        public int EntityId { get; } = -1;

        public PointF Center { get; set; }

        public PointF PlayerLocation => Center;

        public RectangleF SpawnRectangle => new(
            x: MinsSpawnBlock.X,
            y: MinsSpawnBlock.Y,
            width: MaxsSpawnBlock.X - MinsSpawnBlock.X,
            height: MaxsSpawnBlock.Y - MinsSpawnBlock.Y);

        public PlayerZone(int entityId)
        {
            EntityId = entityId;
        }
    }
}