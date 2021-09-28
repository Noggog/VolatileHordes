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

        public IDisposable ApplyTo(
            ZombieGroup group,
            byte range, 
            TimeRange frequency,
            IObservable<Unit>? interrupt = null,
            bool respectRedirect = true)
        {
            interrupt ??= Observable.Return(Unit.Default);

            Logger.Info("Adding Roam AI to {0} with a range of {1} at frequency {2}", group, range, frequency);
            var signal = interrupt
                .Select(_ =>
                {
                    return _timeManager.IntervalWithVariance(
                        frequency,
                        timeSpan => Logger.Info("Will send {0} {1} away in {2}", group, range, timeSpan));
                })
                .Switch();
            if (respectRedirect)
            {
                signal = signal.Merge(
                    Redirect.Signalled
                        .Do(_ => Logger.Verbose("{0} {1} told to redirect by artificial signal.", nameof(RoamControl),
                            group)));
            }

            return signal.SubscribeAsync(async _ =>
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
                    await _zombieControl.SendGroupTowardsDelayed(group, newTarget.Value.ToPoint());
                },
                e => Logger.Error("{0} had update error {1}", nameof(RoamControl), e));
        }
    }
}