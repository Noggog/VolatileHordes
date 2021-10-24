using System.Drawing;
using VolatileHordes.Dto;

namespace VolatileHordes.GUI.ViewModels
{
    public class PlayerVm
    {
        public int EntityId { get; }
        public RectangleF SpawnRectangle { get; private set; }

        public PlayerVm(int entityId)
        {
            EntityId = entityId;
        }

        public void Absorb(PlayerDto dto)
        {
            SpawnRectangle = dto.SpawnRectangle;
        }

        // public Bitmap GetBitmap(ushort width, ushort height)
        // {
        //     
        // }
    }
}