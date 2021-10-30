using System.IO;

namespace VolatileHordes.Dto
{
    public class AllocationStateDto
    {
        public readonly ushort ChunkSize;
        public readonly float[,] Buckets;

        public AllocationStateDto(ushort chunkSize, int width, int height)
        {
            Buckets = new float[width, height];
            ChunkSize = chunkSize;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ChunkSize);
            writer.Write(Buckets.GetLength(0));
            writer.Write(Buckets.GetLength(1));
            for (int x = 0; x < Buckets.GetLength(0); x++)
            {
                for (int y = 0; y < Buckets.GetLength(1); y++)
                {
                    writer.Write(Buckets[x, y]);
                }
            }
        }

        public static AllocationStateDto Deserialize(BinaryReader reader)
        {
            var ret = new AllocationStateDto(
                reader.ReadUInt16(),
                reader.ReadInt32(), 
                reader.ReadInt32());
            for (int x = 0; x < ret.Buckets.GetLength(0); x++)
            {
                for (int y = 0; y < ret.Buckets.GetLength(1); y++)
                {
                    ret.Buckets[x, y] = reader.ReadSingle();
                }
            }

            return ret;
        }
    }
}