using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using UnityEngine;
using VolatileHordes.Randomization;
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
        private readonly RoamControlSettings settings;
        private readonly TimeManager timeManager;
        private readonly SpawningPositions spawningPositions;
        private readonly ZombieControl zombieControl;
        private readonly PointService pointService;
        private readonly RandomSource randomSource;

        public FidgetForward(
            RoamControlSettings settings,
            TimeManager timeManager,
            SpawningPositions spawningPositions,
            ZombieControl zombieControl,
            PointService pointService,
            RandomSource randomSource)
        {
            this.settings = settings;
            this.timeManager = timeManager;
            this.spawningPositions = spawningPositions;
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

                    group.Target.Also(x => Logger.Debug("group.Target:{0}", x));

                    Vector3? newTarget = spawningPositions.GetRandomPointNear(
                        oldTarget == null ? group.Target.Value : pointService.PointDistanceAwayByAngle(
                            group.Target.Value,
                            pointService.RandomlyAdjustAngle(pointService.AngleBetween(group.Target.Value, oldTarget.Value).Log("Angle"), 20).Log("Angle after randomize"),
                            Convert.ToByte(randomSource.NextDouble(range))
                        ),
                        range
                    );

                    if (newTarget == null)
                    {
                        Logger.Warning("Could not find target to instruct group {0} to roam to.", group);
                        return;
                    }
                    oldTarget = group.Target.Value;
                    Logger.Info("Sending group {0} to roam to {1}.", group, newTarget.Value);
                    await zombieControl.SendGroupTowardsDelayed(group, newTarget.Value.ToPoint());
                },
                e => Logger.Error("{0} had update error {1}", nameof(FidgetForward), e));
        }
    }
}