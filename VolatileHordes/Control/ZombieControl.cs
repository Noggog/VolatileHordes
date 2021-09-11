using System.Drawing;
using UnityEngine;
using VolatileHordes.Spawning;

namespace VolatileHordes.Control
{
    public class ZombieControl
    {
        public static readonly ZombieControl Instance = new();

        public void SendZombieTowards(EntityZombie zombie, PointF target)
        {
            var worldTarget = SpawningPositions.Instance.GetWorldVector(target);
            Logger.Debug("Sending zombie towards {0}", worldTarget);
            zombie.SetInvestigatePosition(worldTarget, 6000, false);
        }
    }
}