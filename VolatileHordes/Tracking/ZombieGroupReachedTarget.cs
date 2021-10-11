using System;
using System.Drawing;
using System.Reactive.Linq;
using VolatileHordes.Utility;

namespace VolatileHordes.Tracking
{
    public class ZombieGroupReachedTarget
    {
        private readonly TimeManager _timeManager;

        public ZombieGroupReachedTarget(
            TimeManager timeManager)
        {
            _timeManager = timeManager;
        }

        public IObservable<PointF> WhenReachedTarget(
            ZombieGroup group, 
            float within,
            Func<ZombieGroup, PointF?> locationRetriever)
        {
            return group.FollowTarget()
                .Select<PointF?, IObservable<PointF>>(target =>
                {
                    Logger.Verbose("Monitoring {0} progress to new target {1}", group, target);
                    if (target == null) return Observable.Empty<PointF>();
                    var targetVec = target.Value.ToZeroHeightVector();
                    return _timeManager.Interval(TimeSpan.FromSeconds(5))
                        .Select(_ =>
                        {
                            var locPt = locationRetriever(group);
                            if (locPt == null) return false;
                            var loc = locPt.Value.ToZeroHeightVector();
                            var dist = (loc - targetVec).magnitude;
                            var reachedTarget = dist <= within;
                            if (reachedTarget)
                            {
                                Logger.Verbose("{0} at {1} reached target {2}", group, locPt, target);
                            }
                            else
                            {
                                Logger.Verbose("{0} at {1} did not yet reach target {2}", group, locPt, target);
                            }
                            return reachedTarget;
                        })
                        .Where(x => x)
                        .Take(1)
                        .Select(_ => target.Value);
                })
                .Switch()
                .Publish()
                .RefCount();
        }
    }
}