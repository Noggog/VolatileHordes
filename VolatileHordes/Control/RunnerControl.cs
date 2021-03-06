using System;
using System.Reactive;
using System.Reactive.Linq;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;

namespace VolatileHordes.Control
{
    public class RunnerControl
    {
        private readonly ZombieControl _control;
        private readonly TimeManager _timeManager;
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieGroupReachedTarget _groupReachedTarget;

        public RunnerControl(
            ZombieControl control,
            TimeManager timeManager,
            SpawningPositions spawningPositions,
            ZombieGroupReachedTarget groupReachedTarget)
        {
            _control = control;
            _timeManager = timeManager;
            _spawningPositions = spawningPositions;
            _groupReachedTarget = groupReachedTarget;
        }
        
        public IObservable<Unit> ApplyTo(ZombieGroup group, byte targetPointRadius, byte travelRange)
        {
            return _groupReachedTarget.WhenReachedTarget(group, targetPointRadius, g => g.GetGeneralLocation())
                // Wait 10 seconds to let investigation position settle in
                .Select(target => _timeManager.Timer(TimeSpan.FromSeconds(10))
                    .Select(_ => target))
                .Switch()
                .DoAsync(async target =>
                {
                    var newTarget = _spawningPositions.GetRandomPointNear(target, travelRange);
                    if (newTarget == null)
                    {
                        Logger.Warning($"Could not find target to instruct group {group} to run to.");
                        return;
                    }

                    await _control.SendGroupTowardsDelayed(group, newTarget.Value.ToPoint(), withTargetRandomness: false);
                })
                .Unit();
        }
    }
}