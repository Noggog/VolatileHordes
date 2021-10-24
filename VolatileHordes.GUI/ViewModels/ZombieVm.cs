using System.Drawing;
using Noggog.WPF;
using ReactiveUI.Fody.Helpers;
using VolatileHordes.Dto;

namespace VolatileHordes.GUI.ViewModels
{
    public class ZombieVm : ViewModel
    {
        public int EntityId { get; }
        [Reactive] public PointF Position { get; private set; }
        [Reactive] public PointF Target { get; private set; }

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