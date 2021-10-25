using System;
using System.Reactive;
using System.Reactive.Subjects;
using VolatileHordes.Probability;
using VolatileHordes.Settings.User.Control;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.Control
{
    public class LuckyPlayerRetarget
    {
        private readonly TimeManager _timeManager;
        private readonly RandomSource _randomSource;
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieControl _zombieControl;
        private readonly Percent _percent;

        public LuckyPlayerRetarget(
            TimeManager timeManager,
            RandomSource randomSource,
            LuckyPlayerRetargetSettings settings,
            SpawningPositions spawningPositions,
            ZombieControl zombieControl)
        {
            _timeManager = timeManager;
            _randomSource = randomSource;
            _spawningPositions = spawningPositions;
            _zombieControl = zombieControl;
            _percent = new Percent(settings.ChancePerMinute);
        }
        
        public IObservable<Unit> ApplyTo(ZombieGroup group, out IObservable<Unit> occurred)
        {
            Logger.Info("Adding lucky player retarget AI to {0} at frequency per minute of {1}", group, _percent);
            var occurredSubj = new Subject<Unit>();
            occurred = occurredSubj;
            return _timeManager.Interval(TimeSpan.FromMinutes(1))
                .DoAsync(async () =>
                {
                    if (group.Target == null)
                    {
                        Logger.Warning("{0} did not have a target to base lucky player off of", group);
                        return;
                    }
                    if (!_randomSource.NextChance(_percent)) return;
                    
                    Logger.Info("{0} is getting lucky", group);
                    
                    var player = _spawningPositions.GetNearestPlayer(group.Target.Value);
                    if (player == null)
                    {
                        Logger.Warning("Could not find player to send {0} to", group);
                        return;
                    }
                    
                    occurredSubj.OnNext(Unit.Default);
                    
                    await _zombieControl.SendGroupTowardsDelayed(group, player.location);
                })
                .Unit();
        }
    }
}