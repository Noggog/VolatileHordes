using System.Drawing;
using System.IO;
using VolatileHordes.Dto.Serialization;

namespace VolatileHordes.Dto
{
    public class ZombieDto
    {
        public int EntityId { get; set; }
        public PointF Position { get; set; }
        public PointF Target { get; set; }
        public float Rotation { get; set; }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(EntityId);
            writer.Write(Position);
            writer.Write(Target);
            writer.Write(Rotation);
        }

        public static ZombieDto Deserialize(BinaryReader reader)
        {
            ZombieDto ret = new();
            ret.EntityId = reader.ReadInt32();
            ret.Position = reader.ReadPointF();
            ret.Target = reader.ReadPointF();
            ret.Rotation = reader.ReadSingle();
            return ret;
        }
    }
}