using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Subjects;
using VolatileHordes.Noise;
using VolatileHordes.Randomization;
using VolatileHordes.Settings.User.Control;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.Control
{
    public class NoiseResponderControlFactory
    {
        private readonly RandomSource _randomSource;
        private readonly ZombieControl _zombieControl;
        private readonly NoiseManager _noiseManager;
        private readonly NoiseResponderSettings _noiseResponderSettings;

        public NoiseResponderControlFactory(
            RandomSource randomSource,
            ZombieControl zombieControl,
            NoiseManager noiseManager,
            NoiseResponderSettings noiseResponderSettings)
        {
            _randomSource = randomSource;
            _zombieControl = zombieControl;
            _noiseManager = noiseManager;
            _noiseResponderSettings = noiseResponderSettings;
        }
        
        public NoiseResponderControl Create()
        {
            return new NoiseResponderControl(
                _randomSource,
                _zombieControl,
                _noiseManager.Noise,
                _noiseResponderSettings);
        }
    }
    
    public class NoiseResponderControl
    {
        private readonly RandomSource _randomSource;
        private readonly ZombieControl _zombieControl;
        private readonly IObservable<NoiseEvent> _noises;
        private readonly Subject<Unit> _occurred = new();
        
        private Percent _rememberedVolume;
        private readonly ushort _radius;
        private readonly Percent _maxBaseChance;
        private readonly float _maxVolumeMultiplier;
        private readonly byte _investigateDistance;

        public NoiseResponderControl(
            RandomSource randomSource,
            ZombieControl zombieControl,
            IObservable<NoiseEvent> noises,
            NoiseResponderSettings noiseResponderSettings)
        {
            _randomSource = randomSource;
            _zombieControl = zombieControl;
            _noises = noises;
            _radius = noiseResponderSettings.Radius;
            _maxBaseChance = Percent.FactoryPutInRange(noiseResponderSettings.MaxBaseChance);
            _maxVolumeMultiplier = noiseResponderSettings.MaxVolumeMultiplier;
            _investigateDistance = noiseResponderSettings.InvestigationDistance;
        }
        
        public IDisposable ApplyTo(ZombieGroup group, out IObservable<Unit> occurred)
        {
            occurred = _occurred;
            return _noises.Subscribe(noise =>
            {
                var loc = group.GetGeneralLocation();
                if (loc == null)
                {
                    Logger.Warning("{0} could not respond to noise because it could not get location.", group);
                    return;
                }

                _rememberedVolume += noise.Volume;
                var chance = ChanceToRespond(
                    loc.Value,
                    noise.Origin,
                    _rememberedVolume,
                    _radius,
                    _maxBaseChance,
                    _maxVolumeMultiplier);

                if (!_randomSource.NextChance(chance)) return;

                var target = GetTarget(group.Target, loc.Value, noise.Origin, _investigateDistance);
                
                Logger.Info("{0} responding to noise at {1}, moving to {2}", group, noise.Origin, target);
                _zombieControl.SendGroupTowards(group, target);
                _occurred.OnNext(Unit.Default);
            });
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

            var currentVec = currentTarget.Value.ToZeroHeightVector();
            var noiseVec = noiseOrigin.ToZeroHeightVector();
            
            var diff = noiseVec - currentVec;
            var distance = diff.normalized * investigateDistance;
            if (diff.magnitude < distance.magnitude)
            {
                distance = diff;
            }

            var targetVec = currentVec + diff;
            var targetPt = targetVec.ToPoint();
            return targetPt;
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