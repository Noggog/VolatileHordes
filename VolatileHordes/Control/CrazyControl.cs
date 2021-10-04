using System;
using System.Reactive;
using System.Reactive.Linq;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;

namespace VolatileHordes.Control
{
    public class CrazyControl
    {
        private readonly ZombieControl _control;
        private readonly TimeManager _timeManager;
        private readonly SpawningPositions _spawningPositions;

        public CrazyControl(
            ZombieControl control,
            TimeManager timeManager,
            SpawningPositions spawningPositions)
        {
            _control = control;
            _timeManager = timeManager;
            _spawningPositions = spawningPositions;
        }
        
        public IObservable<Unit> ApplyTo(ZombieGroup group, TimeRange redirectTimeRange, byte travelRange)
        {
            return _timeManager.IntervalWithVariance(redirectTimeRange)
                .Do(_ =>
                {
                    var curLoc = group.GetGeneralLocation();
                    if (curLoc == null)
                    {
                        Logger.Warning("Could not instruct group {0} to redirect, as it had no current location.", group);
                        return;
                    }
                    
                    var newTarget = _spawningPositions.GetRandomPointNear(curLoc.Value, travelRange);
                    if (newTarget == null)
                    {
                        Logger.Warning($"Could not find target to instruct group {group} to redirect to.");
                        return;
                    }

                    _control.SendGroupTowards(group, newTarget.Value.ToPoint());
                })
                .Unit();
        }
    }
}