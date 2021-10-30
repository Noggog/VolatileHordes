using System.Drawing;
using System.IO;

namespace VolatileHordes.Dto.Serialization
{
    public static class BinaryWriterExt
    {
        public static void Write(this BinaryWriter writer, PointF pt)
        {
            writer.Write(pt.X);
            writer.Write(pt.Y);
        }
        
        public static void Write(this BinaryWriter writer, RectangleF rect)
        {
            writer.Write(rect.X);
            writer.Write(rect.Y);
            writer.Write(rect.Width);
            writer.Write(rect.Height);
        }
        
        public static void Write(this BinaryWriter writer, Rectangle rect)
        {
            writer.Write(rect.X);
            writer.Write(rect.Y);
            writer.Write(rect.Width);
            writer.Write(rect.Height);
        }
    }
}