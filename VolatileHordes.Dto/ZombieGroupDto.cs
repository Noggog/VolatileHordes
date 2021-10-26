using System.Collections.Generic;
using System.Drawing;
using System.IO;
using VolatileHordes.Dto.Serialization;

namespace VolatileHordes.Dto
{
    public class ZombieGroupDto
    {
        public int Id { get; set; }
        public PointF Target { get; set; }
        public List<ZombieDto> Zombies { get; set; } = new();

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Target);
            writer.Write(Zombies.Count);
            foreach (var zombie in Zombies)
            {
                zombie.Serialize(writer);
            }
        }

        public static ZombieGroupDto Deserialize(BinaryReader reader)
        {
            var ret = new ZombieGroupDto();

            ret.Id = reader.ReadInt32();
            
            ret.Target = reader.ReadPointF();

            var numZombies = reader.ReadInt32();
            for (int i = 0; i < numZombies; i++)
            {
                ret.Zombies.Add(ZombieDto.Deserialize(reader));
            }

            return ret;
        }
    }
}