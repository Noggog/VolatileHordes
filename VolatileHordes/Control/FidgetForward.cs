using System;
using System.Reactive;
using System.Reactive.Linq;
using UnityEngine;
using VolatileHordes.Settings.User.Control;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.Control
{
    /**
     * Fidgets in a way where the direction can only change by a certain number of degrees at each fidget.
     */
    public class FidgetForward
    {
        private readonly RoamControlSettings _settings;
        private readonly TimeManager _timeManager;
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieControl _zombieControl;

        public FidgetForward(
            RoamControlSettings settings,
            TimeManager timeManager,
            SpawningPositions spawningPositions,
            ZombieControl zombieControl)
        {
            _settings = settings;
            _timeManager = timeManager;
            _spawningPositions = spawningPositions;
            _zombieControl = zombieControl;
        }

        public IDisposable ApplyTo(
            ZombieGroup group,
            IObservable<Unit>? interrupt = null)
        {
            return ApplyTo(group, _settings.Range, new TimeRange(TimeSpan.FromSeconds(_settings.MinSeconds), TimeSpan.FromSeconds(_settings.MaxSeconds)), interrupt);
        }

            public IDisposable ApplyTo(
            ZombieGroup group,
            byte range,
            TimeRange frequency,
            IObservable<Unit>? interrupt = null)
        {
            Vector3? oldTarget = null;

            Logger.Info("Adding FidgetForward AI to {0} with a range of {1} at frequency {2}", group, range, frequency);
            return (interrupt ?? Observable.Empty(Unit.Default))
                .StartWith(Unit.Default)
                .SwitchMap(_ =>
                {
                    return _timeManager.IntervalWithVariance(
                        timeRange : frequency,
                        onNewInterval : timeSpan => Logger.Info("Will fidget {0} in {1}", group, timeSpan));
                })
                .SubscribeAsync(async _ =>
                {
                    if (group.Target == null)
                    {
                        Logger.Warning("Could not instruct group {0} to roam, as it had no current target.", group);
                        return;
                    }

                    Vector3? newTarget;
                    if (oldTarget == null)
                        newTarget = _spawningPositions.GetRandomPointNear(group.Target.Value, range);
                    else
                        newTarget = _spawningPositions.GetRandomPointNear(group.Target.Value.OverPointWithDistance(oldTarget.Value.ToPoint(), range), range);

                    if (newTarget == null)
                    {
                        Logger.Warning("Could not find target to instruct group {0} to roam to.", group);
                        return;
                    }
                    oldTarget = newTarget.Value;
                    Logger.Info("Sending group {0} to roam to {1}.", group, newTarget.Value);
                    await _zombieControl.SendGroupTowardsDelayed(group, newTarget.Value.ToPoint());
                },
                e => Logger.Error("{0} had update error {1}", nameof(RoamControl), e));
        }
    }
}