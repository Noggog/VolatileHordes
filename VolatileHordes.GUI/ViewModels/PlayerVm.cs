using System.Drawing;
using Noggog.WPF;
using ReactiveUI.Fody.Helpers;
using VolatileHordes.Dto;

namespace VolatileHordes.GUI.ViewModels
{
    public class PlayerVm : ViewModel
    {
        public int EntityId { get; }
        [Reactive] public RectangleF Rectangle { get; private set; }
        [Reactive] public RectangleF SpawnRectangle { get; private set; }
        [Reactive] public float Rotation { get; private set; }

        public PlayerVm(int entityId)
        {
            EntityId = entityId;
        }

        public void Absorb(PlayerDto dto)
        {
            SpawnRectangle = dto.SpawnRectangle;
            Rectangle = dto.Rectangle;
            Rotation = dto.Rotation % 360;
        }
    }
}