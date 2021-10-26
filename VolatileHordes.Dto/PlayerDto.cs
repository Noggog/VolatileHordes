using System.Drawing;
using System.IO;
using System.Text;
using VolatileHordes.Dto.Serialization;

namespace VolatileHordes.Dto
{
    public class PlayerDto
    {
        public int EntityId { get; set; }
        public RectangleF SpawnRectangle { get; set; }
        public RectangleF Rectangle { get; set; }
        public float Rotation { get; set; }
        public string Name { get; set; } = string.Empty;

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(EntityId);
            writer.Write(SpawnRectangle);
            writer.Write(Rectangle);
            writer.Write(Rotation);
            writer.Write(Name.Length);
            writer.Write(Encoding.ASCII.GetBytes(Name));
        }

        public static PlayerDto Deserialize(BinaryReader reader)
        {
            var ret = new PlayerDto();
            ret.EntityId = reader.ReadInt32();
            ret.SpawnRectangle = reader.ReadRectangleF();
            ret.Rectangle = reader.ReadRectangleF();
            ret.Rotation = reader.ReadSingle();
            var nameLen = reader.ReadInt32();
            ret.Name = Encoding.ASCII.GetString(reader.ReadBytes(nameLen));
            return ret;
        }
    }
}