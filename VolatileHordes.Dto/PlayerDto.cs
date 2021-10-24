using System.Drawing;
using System.IO;
using VolatileHordes.Dto.Serialization;

namespace VolatileHordes.Dto
{
    public class PlayerDto
    {
        public int EntityId { get; set; }
        public RectangleF SpawnRectangle { get; set; }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(EntityId);
            writer.Write(SpawnRectangle);
        }

        public static PlayerDto Deserialize(BinaryReader reader)
        {
            var ret = new PlayerDto();
            ret.EntityId = reader.ReadInt32();
            ret.SpawnRectangle = reader.ReadRectangleF();
            return ret;
        }
    }
}