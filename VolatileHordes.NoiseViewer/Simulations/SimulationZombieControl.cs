using System.Drawing;
using System.Threading.Tasks;
using VolatileHordes.Control;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Tracking;

namespace VolatileHordes.NoiseViewer.Simulations
{
    public class SimulationZombieControl : IZombieControl
    {
        public void SendZombieTowards(IZombie zombie, PointF target)
        {
            throw new System.NotImplementedException();
        }

        public void SendGroupTowards(IZombieGroup zombieGroup, PointF target, bool withTargetRandomness = true)
        {
            zombieGroup.Target = target;
        }

        public Task SendGroupTowardsDelayed(IZombieGroup zombieGroup, PointF target, bool withTargetRandomness = true)
        {
            throw new System.NotImplementedException();
        }
    }
}