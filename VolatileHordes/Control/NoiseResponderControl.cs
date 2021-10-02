using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Subjects;
using VolatileHordes.Noise;
using VolatileHordes.Randomization;
using VolatileHordes.Settings.User.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;
using ILogger = VolatileHordes.GameAbstractions.ILogger;

namespace VolatileHordes.Control
{
    public class NoiseResponderControlFactory
    {
        private readonly RandomSource _randomSource;
        private readonly IZombieControl _zombieControl;
        private readonly INoiseSource _noiseManager;
        private readonly NoiseResponderSettings _noiseResponderSettings;
        private readonly ILogger _logger;

        public NoiseResponderControlFactory(
            RandomSource randomSource,
            IZombieControl zombieControl,
            INoiseSource noiseManager,
            NoiseResponderSettings noiseResponderSettings,
            ILogger logger)
        {
            _randomSource = randomSource;
            _zombieControl = zombieControl;
            _noiseManager = noiseManager;
            _noiseResponderSettings = noiseResponderSettings;
            _logger = logger;
        }
        
        public NoiseResponderControl Create()
        {
            return new NoiseResponderControl(
                _randomSource,
                _zombieControl,
                _noiseManager.Noise,
                _noiseResponderSettings,
                _logger);
        }
    }
    
    public class NoiseResponderControl
    {
        private readonly RandomSource _randomSource;
        private readonly IZombieControl _zombieControl;
        private readonly IObservable<NoiseEvent> _noises;
        private readonly ILogger _logger;
        private readonly Subject<Unit> _occurred = new();
        
        private Percent _rememberedVolume;
        private readonly ushort _radius;
        private readonly Percent _maxBaseChance;
        private readonly float _maxVolumeMultiplier;
        private readonly byte _investigateDistance;

        public NoiseResponderControl(
            RandomSource randomSource,
            IZombieControl zombieControl,
            IObservable<NoiseEvent> noises,
            NoiseResponderSettings noiseResponderSettings,
            ILogger logger)
        {
            _randomSource = randomSource;
            _zombieControl = zombieControl;
            _noises = noises;
            _logger = logger;
            _radius = noiseResponderSettings.Radius;
            _maxBaseChance = Percent.FactoryPutInRange(noiseResponderSettings.MaxBaseChance);
            _maxVolumeMultiplier = noiseResponderSettings.MaxVolumeMultiplier;
            _investigateDistance = noiseResponderSettings.InvestigationDistance;
        }
        
        public IDisposable ApplyTo(IZombieGroup group, out IObservable<Unit> occurred)
        {
            occurred = _occurred;
            return _noises.Subscribe(noise =>
            {
                var loc = group.GetGeneralLocation();
                if (loc == null)
                {
                    _logger.Warning("{0} could not respond to noise because it could not get location.", group);
                    return;
                }

                var dist = loc.Value.AbsDistance(noise.Origin);
                if (dist > _radius) return;

                var chance = GetChanceToRespond(loc.Value, noise.Origin, noise.Volume);
                _rememberedVolume += noise.Volume;

                if (!_randomSource.NextChance(chance)) return;

                var target = GetTarget(group.Target, loc.Value, noise.Origin, _investigateDistance);
                
                _logger.Info("{0} responding to noise at {1}, moving to {2}", group, noise.Origin, target);
                _zombieControl.SendGroupTowards(group, target);
                _occurred.OnNext(Unit.Default);
            });
        }

        public Percent GetChanceToRespond(PointF loc, PointF noiseOrigin, Percent incomingVolume)
        {
            return ChanceToRespond(
                loc,
                noiseOrigin,
                _rememberedVolume + incomingVolume,
                _radius,
                _maxBaseChance,
                _maxVolumeMultiplier);
        }

        public static PointF GetTarget(
            PointF? currentTarget, 
            PointF curLoc, 
            PointF noiseOrigin,
            byte investigateDistance)
        {
            if (!currentTarget.HasValue || curLoc.IsTargetAwayFrom(currentTarget.Value, noiseOrigin))
            {
                currentTarget = curLoc;
            }

            var currentVec = currentTarget.Value.ToVector();
            var noiseVec = noiseOrigin.ToVector();
            
            var diff = noiseVec - currentVec;
            if (diff.Length.EqualsWithin(0))
            {
                return currentVec.ToPoint();
            }
            var normalizedDiff = diff;
            normalizedDiff.Normalize();
            var distance = normalizedDiff * investigateDistance;
            if (diff.Length < distance.Length)
            {
                distance = diff;
            }

            var targetVec = currentVec + distance;
            return targetVec.ToPoint();
        }
        
        public static Percent ChanceToRespond(
            PointF zombieLocation,
            PointF originLocation,
            Percent rememberedVolume,
            ushort radius,
            Percent maxBaseChance,
            float maxVolumeMultiplier)
        {
            var pctDistance = zombieLocation.PercentAwayFrom(originLocation, radius).Inverse;

            var chance = maxBaseChance * pctDistance;

            var volumeMult = 1f + rememberedVolume * maxVolumeMultiplier;

            chance = Percent.FactoryPutInRange(chance * volumeMult);

            return chance;
        }
    }
}