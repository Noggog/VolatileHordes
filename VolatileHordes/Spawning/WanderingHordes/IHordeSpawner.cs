using System.Drawing;
using System.Threading.Tasks;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public interface IHordeSpawner
    {
        Task Spawn(ushort size, Point chunkPoint);
    }
}