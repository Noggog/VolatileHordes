using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using VolatileHordes.Core.ObservableTransforms;
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
        private readonly TemporaryAiShutoff _temporaryAiShutoff;
        private readonly NoiseResponderSettings _noiseResponderSettings;
        private readonly ILogger _logger;

        public NoiseResponderControlFactory(
            RandomSource randomSource,
            IZombieControl zombieControl,
            INoiseSource noiseManager,
            TemporaryAiShutoff temporaryAiShutoff,
            NoiseResponderSettings noiseResponderSettings,
            ILogger logger)
        {
            _randomSource = randomSource;
            _zombieControl = zombieControl;
            _noiseManager = noiseManager;
            _temporaryAiShutoff = temporaryAiShutoff;
            _noiseResponderSettings = noiseResponderSettings;
            _logger = logger;
        }
        
        public NoiseResponderControl Create()
        {
            return new NoiseResponderControl(
                _randomSource,
                _zombieControl,
                _noiseManager,
                _noiseResponderSettings,
                _temporaryAiShutoff,
                _logger);
        }
    }

    public class NoiseResponderControl
    {
        private readonly RandomSource _randomSource;
        private readonly IZombieControl _zombieControl;
        private readonly INoiseSource _noises;

        private readonly TemporaryShutoffTransformation _temporaryShutoffTransformation;

        private readonly ILogger _logger;
        private readonly Subject<Unit> _occurred = new();

        private Percent _rememberedVolume;
        private readonly ushort _radius;
        private readonly byte _investigateDistanceMin;
        private readonly byte _investigateDistanceBonus;
        private readonly float _noiseLostPerTick;

        public IObservable<Unit> Occurred => _occurred;

        public NoiseResponderControl(
            RandomSource randomSource,
            IZombieControl zombieControl,
            INoiseSource noises,
            NoiseResponderSettings noiseResponderSettings,
            TemporaryAiShutoff temporaryAiShutoff,
            ILogger logger)
        {
            _randomSource = randomSource;
            _zombieControl = zombieControl;
            _noises = noises;
            _temporaryShutoffTransformation = new(_occurred, temporaryAiShutoff);
            _logger = logger;
            _radius = noiseResponderSettings.Radius;
            _investigateDistanceMin = Math.Min(noiseResponderSettings.InvestigationDistanceMin,
                noiseResponderSettings.InvestigationDistanceMax);
            var max = Math.Max(noiseResponderSettings.InvestigationDistanceMin,
                noiseResponderSettings.InvestigationDistanceMax);
            _investigateDistanceBonus = (byte)(max - _investigateDistanceMin);
            _noiseLostPerTick = noiseResponderSettings.NoiseLostPerTwoSeconds;
        }

        public IObservable<Unit> ApplyTo(ZombieGroup group)
        {
            return Observable.Merge(
                _noises.NoiseReduction
                    .Do(_ =>
                    {
                        _rememberedVolume = Percent.FactoryPutInRange(_rememberedVolume.Value - _noiseLostPerTick);
                    })
                    .Unit(),
                _noises.Noise.Do(noise =>
                    {
                        if (group.Empty) return;
                        var loc = group.GetLocationClosestTo(noise.Origin);
                        if (loc == null)
                        {
                            _logger.Warning("{0} could not respond to noise because it could not get location.", group);
                            return;
                        }

                        var curDistToOrigin = loc.Value.AbsDistance(noise.Origin);
                        if (curDistToOrigin > _radius) return;

                        _rememberedVolume = Percent.FactoryPutInRange(_rememberedVolume.Value + noise.Volume);

                        var chance = GetChanceToRespond(loc.Value, noise.Origin);

                        _logger.Verbose("{0} with remembered volume {1} has a chance of {2} to respond to noise at {3}",
                            group, _rememberedVolume, chance, noise.Origin);

                        if (!_randomSource.NextChance(chance)) return;

                        var target = GetTarget(loc.Value, noise.Origin, GetInvestigateDistance());

                        _logger.Debug("{0} with remembered volume {1} responding to noise at {2}, moving to {3}", group,
                            _rememberedVolume, noise.Origin, target);
                        _zombieControl.SendGroupTowards(group, target);
                        _occurred.OnNext(Unit.Default);
                    })
                    .Unit());
        }

        public Percent GetChanceToRespond(PointF loc, PointF noiseOrigin)
        {
            return ChanceToRespond(
                loc,
                noiseOrigin,
                _rememberedVolume,
                _radius);
        }

        private float GetInvestigateDistance()
        {
            return _investigateDistanceMin + _investigateDistanceBonus * _rememberedVolume;
        }

        public static PointF GetTarget(
            PointF curLoc,
            PointF noiseOrigin,
            float investigateDistance)
        {
            var currentVec = curLoc.ToZeroHeightVector();
            var noiseVec = noiseOrigin.ToZeroHeightVector();

            var diff = noiseVec - currentVec;
            if (diff.magnitude.EqualsWithin(0))
            {
                return currentVec.ToPoint();
            }

            var normalizedDiff = diff;
            normalizedDiff.Normalize();
            var distance = normalizedDiff * investigateDistance;
            if (diff.magnitude < distance.magnitude)
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
            ushort radius)
        {
            var pctDistance = zombieLocation.PercentAwayFrom(originLocation, radius).Inverse;

            var chance = Percent.FactoryPutInRange(pctDistance * rememberedVolume);

            return chance;
        }
        
        private class TemporaryShutoffTransformation : IObservableTransformation<Unit, Unit, TimeSpan, string>
        {
            private readonly IObservable<Unit> _occurred;
            private readonly TemporaryAiShutoff _aiShutoff;

            public TemporaryShutoffTransformation(
                IObservable<Unit> occurred,
                TemporaryAiShutoff aiShutoff)
            {
                _occurred = occurred;
                _aiShutoff = aiShutoff;
            }
            
            public IObservable<Unit> Transform(IObservable<Unit> obs, TimeSpan timeToShutOff, string componentName)
            {
                return obs
                    .Compose(_aiShutoff, _occurred, timeToShutOff, componentName, "noticed noise");
            }
        }

        public IObservableTransformation<Unit, Unit, TimeSpan, string> TemporaryShutoffOnNoise()
        {
            return _temporaryShutoffTransformation;
        }
    }
}