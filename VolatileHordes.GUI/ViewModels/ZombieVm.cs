using System.Drawing;
using Noggog.WPF;
using VolatileHordes.Dto;

namespace VolatileHordes.GUI.ViewModels
{
    public class ZombieVm : ViewModel
    {
        public int EntityId { get; }
        public PointF Position { get; private set; }
        public PointF Target { get; private set; }

        public ZombieVm(int entityId)
        {
            EntityId = entityId;
        }

        public void Absorb(ZombieDto dto)
        {
            Position = dto.Position;
            Target = dto.Target;
        }
    }
}