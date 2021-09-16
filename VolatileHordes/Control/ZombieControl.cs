using System.Drawing;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Spawning;

namespace VolatileHordes.Control
{
    public class ZombieControl
    {
        private readonly SpawningPositions _spawningPositions;

        public ZombieControl(SpawningPositions spawningPositions)
        {
            _spawningPositions = spawningPositions;
        }
        
        public void SendZombieTowards(IZombie zombie, PointF target)
        {
            var worldTarget = _spawningPositions.GetWorldVector(target);
            Logger.Debug("Sending zombie towards {0}", worldTarget);
            zombie.SendTowards(worldTarget);
        }

        public void SendGroupTowards(ZombieGroup zombieGroup, PointF target)
        {
            var worldTarget = _spawningPositions.GetWorldVector(target);
            Logger.Debug("Sending {0} zombies towards {1}", zombieGroup.Zombies.Count, worldTarget);
            zombieGroup.Target = target;
            foreach (var zombie in zombieGroup.Zombies)
            {
                zombie.SendTowards(worldTarget);
            }
        }
    }
}