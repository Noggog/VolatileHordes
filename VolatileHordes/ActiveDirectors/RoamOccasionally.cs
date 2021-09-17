using System;
using VolatileHordes.Control;
using VolatileHordes.Spawning;

namespace VolatileHordes.ActiveDirectors
{
    public class RoamOccasionally
    {
        private readonly TimeManager _timeManager;
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieControl _zombieControl;

        public RoamOccasionally(
            TimeManager timeManager, 
            SpawningPositions spawningPositions,
            ZombieControl zombieControl)
        {
            _timeManager = timeManager;
            _spawningPositions = spawningPositions;
            _zombieControl = zombieControl;
        }

        public void ApplyTo(ZombieGroup group, byte range, TimeSpan timeSpan, double variance)
        {
            group.AddForDisposal(
                _timeManager.IntervalWithVariance(timeSpan, variance)
                    .Subscribe(_ =>
                    {
                        if (group.Target == null)
                        {
                            Logger.Warning($"Could not instruct group {group.Id} to roam, as it had no current target.");
                            return;
                        }

                        var newTarget = _spawningPositions.GetRandomPointNear(group.Target.Value, range);
                        if (newTarget == null)
                        {
                            Logger.Warning($"Could not find target to instruct group {group.Id} to roam to.");
                            return;
                        }
                        
                        Logger.Info($"Sending group {group.Id} to roam to {newTarget.Value}.");
                        _zombieControl.SendGroupTowards(group, newTarget.Value.ToPoint());
                    }));
        }
    }
}