using System;
using VolatileHordes.Settings.User.Control;
using VolatileHordes.Tracking;

namespace VolatileHordes.Control
{
    public class RoamFarOccasionally
    {
        private readonly RoamControlSettings _settings;
        private readonly RoamControl _roamControl;

        public RoamFarOccasionally(
            RoamControlSettings settings,
            RoamControl roamControl)
        {
            _settings = settings;
            _roamControl = roamControl;
        }

        public IDisposable ApplyTo(ZombieGroup group)
        {
            return _roamControl.ApplyTo(
                group,
                _settings.Range,
                new TimeRange(TimeSpan.FromSeconds(_settings.MinSeconds), TimeSpan.FromSeconds(_settings.MaxSeconds)));
        }
    }
}