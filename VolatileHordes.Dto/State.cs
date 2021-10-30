using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using VolatileHordes.Dto.Serialization;

namespace VolatileHordes.Dto
{
    public class State
    {
        public ZombieLimitsDto Limits { get; set; } = new();
        public AllocationStateDto AllocationState { get; set; } = null!;
        public List<ZombieGroupDto> ZombieGroups { get; set; } = new();
        public List<PlayerDto> Players { get; set; } = new();
        public ushort NoiseRadius { get; set; }
        public Rectangle WorldRect { get; set; }

        public void Serialize(BinaryWriter stream)
        {
            stream.Write(1753);
            Limits.Serialize(stream);
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
            AllocationState.Serialize(stream);
            stream.Write(NoiseRadius);
            stream.Write(WorldRect);
        }

        public static State Deserialize(BinaryReader reader)
        {
            var ret = new State();
            var header = reader.ReadInt32();
            if (header != 1753)
            {
                throw new ArgumentException("Malformed state");
            }
            ret.Limits = ZombieLimitsDto.Deserialize(reader);
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

            ret.AllocationState = AllocationStateDto.Deserialize(reader);
            ret.NoiseRadius = reader.ReadUInt16();
            ret.WorldRect = reader.ReadRectangle();

            return ret;
        }
    }
}
