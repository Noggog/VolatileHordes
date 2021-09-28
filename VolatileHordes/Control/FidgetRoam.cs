using System;
using System.Reactive;
using VolatileHordes.Settings.User.Control;
using VolatileHordes.Tracking;

namespace VolatileHordes.Control
{
    public class FidgetRoam
    {
        private readonly RoamControlSettings _settings;
        private readonly RoamControl _roamControl;

        public FidgetRoam(
            RoamControlSettings settings,
            RoamControl roamControl)
        {
            _settings = settings;
            _roamControl = roamControl;
        }

        public IDisposable ApplyTo(ZombieGroup group, IObservable<Unit>? interrupt = null)
        {
            return _roamControl.ApplyTo(
                group,
                _settings.Range,
                new TimeRange(TimeSpan.FromSeconds(_settings.MinSeconds), TimeSpan.FromSeconds(_settings.MaxSeconds)),
                interrupt,
                respectRedirect: false);
        }
    }
}