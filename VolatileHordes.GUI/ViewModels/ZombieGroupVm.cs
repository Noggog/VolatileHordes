using System.Drawing;
using DynamicData;
using VolatileHordes.Dto;
using VolatileHordes.GUI.Extensions;

namespace VolatileHordes.GUI.ViewModels
{
    public class ZombieGroupVm
    {
        public int Id { get; }

        public PointF Target { get; private set; }
        
        private SourceCache<ZombieVm, int> _zombies = new(x => x.EntityId);

        public ZombieGroupVm(int id)
        {
            Id = id;
        }

        public void Absorb(ZombieGroupDto dto)
        {
            Target = dto.Target;
            _zombies.AbsorbIn(
                dto.Zombies, 
                o => o.EntityId, 
                (k) => new ZombieVm(k),
                (vm, dto) => vm.Absorb(dto));
        }
    }
}