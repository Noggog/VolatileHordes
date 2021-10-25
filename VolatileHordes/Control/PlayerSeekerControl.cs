using System;
using System.Reactive;
using System.Reactive.Linq;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;

namespace VolatileHordes.Control
{
    public class PlayerSeekerControl
    {
        private readonly TimeManager _timeManager;
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieControl _zombieControl;

        public PlayerSeekerControl(
            TimeManager timeManager,
            SpawningPositions spawningPositions,
            ZombieControl zombieControl)
        {
            _timeManager = timeManager;
            _spawningPositions = spawningPositions;
            _zombieControl = zombieControl;
        }

        public IObservable<Unit> ApplyTo(ZombieGroup group, TimeSpan frequency)
        {
            Logger.Info("Adding Seeker AI to {0} at frequency {1}", group, frequency);
            return _timeManager.Interval(frequency)
                .Do(_ =>
                {
                    if (group.Target == null)
                    {
                        Logger.Warning("{0} did not have a target to measure from", group);
                        return;
                    }
                    
                    var player = _spawningPositions.GetNearestPlayer(group.Target.Value);
                    if (player == null)
                    {
                        Logger.Warning("Could not find player to send {0} to", group);
                        return;
                    }
                    
                    _zombieControl.SendGroupTowards(group, player.location);
                })
                .Unit();
        }
    }
}