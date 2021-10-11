using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Threading.Tasks;
using UniLinq;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Probability;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;

namespace VolatileHordes.Control
{
    public interface IZombieControl
    {
        void SendZombieTowards(IZombie zombie, PointF target);
        void SendGroupTowards(ZombieGroup zombieGroup, PointF target, bool withTargetRandomness = true);
        Task SendGroupTowardsDelayed(ZombieGroup zombieGroup, PointF target, bool withTargetRandomness = true);
    }

    public class ZombieControl : IZombieControl
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
            Logger.Debug("Sending zombie towards {0}", target);
            zombie.SendTowards(target);
        }

        public void SendGroupTowards(ZombieGroup zombieGroup, PointF target, bool withTargetRandomness = true)
        {
            Logger.Debug("Will send {0} zombies towards {1}", zombieGroup.Count, target);
            zombieGroup.Target = target;
            foreach (var zombie in zombieGroup)
            {
                if (withTargetRandomness)
                {
                    target = _spawningPositions.GetRandomPointNear(target, 5)?.ToPoint() ?? target;
                    Logger.Verbose(".. With randomness, will send 1 zombie of the group towards {0}", target);
                }
                zombie.SendTowards(target);
            }
        }

        public async Task SendGroupTowardsDelayed(ZombieGroup zombieGroup, PointF target, bool withTargetRandomness = true)
        {
            Logger.Debug("Will send {0} zombies towards {1}", zombieGroup.Count, target);
            zombieGroup.Target = target;
            await Task.WhenAll(zombieGroup.Select(async zombie =>
            {
                if (withTargetRandomness)
                {
                    target = _spawningPositions.GetRandomPointNear(target, 5)?.ToPoint() ?? target;
                    Logger.Verbose(".. With randomness, will send 1 zombie of the group towards {0}", target);
                }

                await _timeManager.Timer(TimeSpan.FromSeconds(_randomSource.NextDouble(5)))
                    .Do(x =>
                    {
                        Logger.Verbose("Sending 1 zombie of the group towards {0}", target);
                        zombie.SendTowards(target);
                    });
            }));
        }
    }
}