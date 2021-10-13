using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using VolatileHordes.Probability;
using VolatileHordes.Settings.User.Control;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.Control
{
    /**
     * Fidgets in a way where the direction can only change by a certain number of degrees at each fidget.
     */
    public class FidgetForwardControl
    {
        private readonly RoamControlSettings settings;
        private readonly TimeManager timeManager;
        private readonly ZombieControl zombieControl;
        private readonly PointService pointService;
        private readonly RandomSource randomSource;

        public FidgetForwardControl(
            RoamControlSettings settings,
            TimeManager timeManager,
            ZombieControl zombieControl,
            PointService pointService,
            RandomSource randomSource)
        {
            this.settings = settings;
            this.timeManager = timeManager;
            this.zombieControl = zombieControl;
            this.pointService = pointService;
            this.randomSource = randomSource;
        }

        public IDisposable ApplyTo(
            ZombieGroup group,
            IObservable<Unit>? restart = null)
        {
            return ApplyTo(group, settings.Range, new TimeRange(TimeSpan.FromSeconds(settings.MinSeconds), TimeSpan.FromSeconds(settings.MaxSeconds)), restart);
        }

        public IDisposable ApplyTo(
            ZombieGroup group,
            byte range,
            TimeRange frequency,
            IObservable<Unit>? restart = null)
        {
            PointF? oldTarget = null;

            Logger.Info("Adding FidgetForward AI to {0} with a range of {1} at frequency {2}", group, range, frequency);
            return (restart ?? Observable.Empty(Unit.Default))
                .StartWith(Unit.Default)
                .SwitchMap(_ =>
                {
                    return timeManager.IntervalWithVariance(
                        timeRange: frequency,
                        onNewInterval: timeSpan => Logger.Info("Will fidget {0} in {1}", group, timeSpan));
                })
                .SubscribeAsync(async _ =>
                {
                    if (group.Target == null)
                    {
                        Logger.Warning("Could not instruct group {0} to roam, as it had no current target.", group);
                        return;
                    }

                    var newTarget = pointService.PointDistanceAwayByAngle(
                        point: group.Target.Value,
                        angle: oldTarget == null
                            ? 360 * randomSource.NextDouble()
                            : pointService.RandomlyAdjustAngle(
                                pointService.AngleBetween(
                                    oldTarget.Value,
                                    group.Target.Value
                                ),
                                20
                            ),
                        distance: range
                    );

                    oldTarget = group.Target.Value;
                    Logger.Info("Sending group {0} to roam to {1}.", group, newTarget);
                    await zombieControl.SendGroupTowardsDelayed(group, newTarget);
                },
                e => Logger.Error("{0} had update error {1}", nameof(FidgetForwardControl), e));
        }
    }
}