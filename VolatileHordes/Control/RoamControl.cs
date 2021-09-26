using System;
using System.Reactive;
using System.Reactive.Linq;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.Control
{
    public class RoamControl
    {
        private readonly TimeManager _timeManager;
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieControl _zombieControl;

        public Signal Redirect { get; } = new();

        public RoamControl(
            TimeManager timeManager,
            SpawningPositions spawningPositions,
            ZombieControl zombieControl)
        {
            _timeManager = timeManager;
            _spawningPositions = spawningPositions;
            _zombieControl = zombieControl;
        }

        public IDisposable ApplyTo(ZombieGroup group, byte range, TimeRange frequency, IObservable<Unit>? interrupt = null)
        {
            interrupt ??= Observable.Return(Unit.Default);
            
            Logger.Info("Adding Roam AI to {0} with a range of {1} at frequency {2}", group, range, frequency);
            return interrupt
                .Select(_ =>
                {
                    return _timeManager.IntervalWithVariance(
                        frequency,
                        timeSpan => Logger.Info("Will send {0} {1} away in {2}", group, range, timeSpan));
                })
                .Switch()
                .Merge(Redirect.Signalled)
                .Subscribe(_ =>
                {
                    if (group.Target == null)
                    {
                        Logger.Warning("Could not instruct group {0} to roam, as it had no current target.", group);
                        return;
                    }

                    var newTarget = _spawningPositions.GetRandomPointNear(group.Target.Value, range);
                    if (newTarget == null)
                    {
                        Logger.Warning("Could not find target to instruct group {0} to roam to.", group);
                        return;
                    }

                    Logger.Info("Sending group {0} to roam to {1}.", group, newTarget.Value);
                    _zombieControl.SendGroupTowards(group, newTarget.Value.ToPoint());
                },
                    e => Logger.Error("{0} had update error {1}", nameof(RoamControl), e));
        }
    }
}