using System;
using System.Collections.Generic;
using System.IO;

namespace VolatileHordes.Dto
{
    public class State
    {
        public List<ZombieGroupDto> ZombieGroups { get; set; } = new();
        public List<PlayerDto> Players { get; set; } = new();

        public void Serialize(BinaryWriter stream)
        {
            stream.Write(1753);
            stream.Write(ZombieGroups.Count);
            foreach (var groupDto in ZombieGroups)
            {
                groupDto.Serialize(stream);
            }
            stream.Write(Players.Count);
            foreach (var player in Players)
            {
                player.Serialize(stream);
            }
        }

        public static State Deserialize(BinaryReader reader)
        {
            var ret = new State();
            var header = reader.ReadInt32();
            if (header != 1753)
            {
                throw new ArgumentException("Malformed state");
            }
            var zombieGroupCount = reader.ReadInt32();
            for (int i = 0; i < zombieGroupCount; i++)
            {
                ret.ZombieGroups.Add(ZombieGroupDto.Deserialize(reader));
            }
            var playerGroupCount = reader.ReadInt32();
            for (int i = 0; i < playerGroupCount; i++)
            {
                ret.Players.Add(PlayerDto.Deserialize(reader));
            }

            return ret;
        }
    }
}
