using System;
using System.Drawing;
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
        private readonly GroupReachedTarget _groupReachedTarget;

        public RunnerControl(
            ZombieControl control,
            TimeManager timeManager,
            SpawningPositions spawningPositions,
            GroupReachedTarget groupReachedTarget)
        {
            _control = control;
            _timeManager = timeManager;
            _spawningPositions = spawningPositions;
            _groupReachedTarget = groupReachedTarget;
        }
        
        public IDisposable ApplyTo(ZombieGroup group, byte targetPointRadius, byte travelRange)
        {
            return _groupReachedTarget.WhenReachedTarget(group, targetPointRadius, g => g.GetGeneralLocation())
                // Wait 10 seconds to let investigation position settle in
                .Select(target => _timeManager.Timer(TimeSpan.FromSeconds(10))
                    .Select(_ => target))
                .Switch()
                .Subscribe(target =>
                {
                    var newTarget = _spawningPositions.GetRandomPointNear(target, travelRange);
                    if (newTarget == null)
                    {
                        Logger.Warning($"Could not find target to instruct group {group} to roam to.");
                        return;
                    }

                    _control.SendGroupTowards(group, newTarget.Value.ToPoint(), withRandomness: false);
                });
        }
    }
}