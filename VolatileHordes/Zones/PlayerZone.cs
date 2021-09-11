using System.Drawing;
using UnityEngine;
using VolatileHordes.Randomization;
using VolatileHordes.Spawning;

namespace VolatileHordes.Zones
{
    public class PlayerZone
    {
        public bool Valid { get; set; } = true;
        public PointF Mins { get; set; } = PointF.Empty;
        public PointF Maxs { get; set; } = PointF.Empty;
        public PointF MinsSpawnBlock { get; set; } = PointF.Empty;
        public PointF MaxsSpawnBlock { get; set; } = PointF.Empty;
        public int EntityId { get; } = -1;

        public PointF Center { get; set; }

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