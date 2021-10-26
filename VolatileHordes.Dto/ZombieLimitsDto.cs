using System.IO;

namespace VolatileHordes.Dto
{
    public class ZombieLimitsDto
    {
        public int GameMaximum { get; set; }
        public int CurrentNumber { get; set; }
        public int DesiredMaximum { get; set; }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(GameMaximum);
            writer.Write(CurrentNumber);
            writer.Write(DesiredMaximum);
        }

        public static ZombieLimitsDto Deserialize(BinaryReader reader)
        {
            return new ZombieLimitsDto()
            {
                GameMaximum = reader.ReadInt32(),
                CurrentNumber = reader.ReadInt32(),
                DesiredMaximum = reader.ReadInt32(),
            };
        }
    }
}