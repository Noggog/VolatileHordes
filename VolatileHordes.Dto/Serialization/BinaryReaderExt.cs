using System.Drawing;
using System.IO;

namespace VolatileHordes.Dto.Serialization
{
    public static class BinaryReaderExt
    {
        public static PointF ReadPointF(this BinaryReader reader)
        {
            return new PointF(reader.ReadSingle(), reader.ReadSingle());
        }
        
        public static RectangleF ReadRectangleF(this BinaryReader reader)
        {
            return new RectangleF(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
        }
        
        public static Rectangle ReadRectangle(this BinaryReader reader)
        {
            return new Rectangle(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32());
        }
    }
}