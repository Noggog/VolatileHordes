using System.Drawing;
using UnityEngine;
using VolatileHordes.Randomization;
using VolatileHordes.Spawning;

namespace VolatileHordes.Zones
{
    public class PlayerZone
    {
        public bool Valid { get; set; } = true;
        public Vector3 Mins { get; set; }  = Vector3.zero;
        public Vector3 Maxs { get; set; } = Vector3.zero;
        public Vector3 MinsSpawnBlock { get; set; }  = Vector3.zero;
        public Vector3 MaxsSpawnBlock { get; set; }  = Vector3.zero;
        public int EntityId { get; } = -1;

        public Vector3 Center { get; set; }

        public RectangleF SpawnRectangle => new(
            x: MinsSpawnBlock.x,
            y: MinsSpawnBlock.z,
            width: MaxsSpawnBlock.x - MinsSpawnBlock.x,
            height: MaxsSpawnBlock.z - MinsSpawnBlock.z);

        public PlayerZone(int entityId)
        {
            EntityId = entityId;
        }
    }
}