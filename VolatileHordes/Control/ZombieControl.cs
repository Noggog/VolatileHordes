using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Threading.Tasks;
using UniLinq;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Randomization;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;

namespace VolatileHordes.Control
{
    public class ZombieControl
    {
        private readonly SpawningPositions _spawningPositions;
        private readonly TimeManager _timeManager;
        private readonly RandomSource _randomSource;

        public ZombieControl(SpawningPositions spawningPositions, TimeManager timeManager, RandomSource randomSource)
        {
            _spawningPositions = spawningPositions;
            _timeManager = timeManager;
            _randomSource = randomSource;
        }
        
        public void SendZombieTowards(IZombie zombie, PointF target)
        {
            var worldTarget = _spawningPositions.GetWorldVector(target);
            Logger.Debug("Sending zombie towards {0}", worldTarget);
            zombie.SendTowards(worldTarget);
        }

        public void SendGroupTowards(ZombieGroup zombieGroup, PointF target, bool withTargetRandomness = true)
        {
            var worldTarget = _spawningPositions.GetWorldVector(target);
            Logger.Debug("Will send {0} zombies towards {1}", zombieGroup.Zombies.Count, worldTarget);
            zombieGroup.Target = target;
            foreach (var zombie in zombieGroup.Zombies)
            {
                if (withTargetRandomness)
                {
                    worldTarget = _spawningPositions.GetRandomPointNear(target, 5) ?? worldTarget;
                    Logger.Verbose(".. With randomness, will send 1 zombie of the group towards {0}", worldTarget);
                }
                zombie.SendTowards(worldTarget);
            }
        }

        public async Task SendGroupTowardsDelayed(ZombieGroup zombieGroup, PointF target, bool withTargetRandomness = true)
        {
            var worldTarget = _spawningPositions.GetWorldVector(target);
            Logger.Debug("Will send {0} zombies towards {1}", zombieGroup.Zombies.Count, worldTarget);
            zombieGroup.Target = target;
            await Task.WhenAll(zombieGroup.Zombies.Select(async zombie =>
            {
                if (withTargetRandomness)
                {
                    worldTarget = _spawningPositions.GetRandomPointNear(target, 5) ?? worldTarget;
                    Logger.Verbose(".. With randomness, will send 1 zombie of the group towards {0}", worldTarget);
                }

                await _timeManager.Timer(TimeSpan.FromSeconds(_randomSource.NextDouble(5)))
                    .Do(x =>
                    {
                        Logger.Verbose("Sending 1 zombie of the group towards {0}", worldTarget);
                        zombie.SendTowards(worldTarget);
                    });
            }));
        }
    }
}