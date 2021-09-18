using System;
using System.Reactive.Linq;
using VolatileHordes.Spawning;
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

        public IDisposable ApplyTo(ZombieGroup group, byte range, TimeRange frequency)
        {
            Logger.Info("Adding roam AI to {0} with a range of {1} at frequency {2}", group, range, frequency);
            return _timeManager.IntervalWithVariance(
                    frequency,
                    timeSpan => Logger.Info($"Will send {group} somewhere in {timeSpan}"))
                .Unit()
                .Merge(Redirect.Signalled)
                .Subscribe(_ =>
                {
                    if (group.Target == null)
                    {
                        Logger.Warning($"Could not instruct group {group} to roam, as it had no current target.");
                        return;
                    }

                    var newTarget = _spawningPositions.GetRandomPointNear(group.Target.Value, range);
                    if (newTarget == null)
                    {
                        Logger.Warning($"Could not find target to instruct group {group} to roam to.");
                        return;
                    }

                    Logger.Info($"Sending group {group} to roam to {newTarget.Value}.");
                    _zombieControl.SendGroupTowards(group, newTarget.Value.ToPoint());
                });
        }
    }
}