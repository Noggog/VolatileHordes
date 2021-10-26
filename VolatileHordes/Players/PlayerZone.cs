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
        public Vector3 Rotation { get; set; }

        public PointF Center { get; set; }

        public PointF PlayerLocation => Center;
        
        public IPlayer Player { get; }

        public RectangleF SpawnRectangle => new(
            x: MinsSpawnBlock.X,
            y: MinsSpawnBlock.Y,
            width: MaxsSpawnBlock.X - MinsSpawnBlock.X,
            height: MaxsSpawnBlock.Y - MinsSpawnBlock.Y);

        public RectangleF Rectangle => new(
            x: Mins.X,
            y: Mins.Y,
            width: Maxs.X - Mins.X,
            height: Maxs.Y - Mins.Y);

        public PlayerZone(IPlayer player)
        {
            Player = player;
        }
    }
}