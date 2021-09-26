using System.Drawing;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;

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

        public void SendGroupTowards(ZombieGroup zombieGroup, PointF target, bool withRandomness = true)
        {
            var worldTarget = _spawningPositions.GetWorldVector(target);
            Logger.Debug("Sending {0} zombies towards {1}", zombieGroup.Zombies.Count, worldTarget);
            zombieGroup.Target = target;
            foreach (var zombie in zombieGroup.Zombies)
            {
                var worldTargetRedefined = worldTarget;
                if (withRandomness)
                {
                    Logger.Debug(".. With randomness, sending 1 zombie of the group towards {0}", worldTargetRedefined);
                    worldTargetRedefined = _spawningPositions.GetRandomPointNear(target, 5) ?? worldTarget;
                }
                zombie.SendTowards(worldTargetRedefined);
            }
        }
    }
}