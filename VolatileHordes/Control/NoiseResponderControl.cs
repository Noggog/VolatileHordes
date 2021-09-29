using System;
using System.Reactive;
using System.Reactive.Subjects;
using VolatileHordes.Noise;
using VolatileHordes.Randomization;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;
using VolatileHordes.Utility;

namespace VolatileHordes.Control
{
    public class NoiseResponderControlFactory
    {
        private readonly RandomSource _randomSource;
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieControl _zombieControl;
        private readonly NoiseManager _noiseManager;

        public NoiseResponderControlFactory(
            RandomSource randomSource,
            SpawningPositions spawningPositions,
            ZombieControl zombieControl,
            NoiseManager noiseManager)
        {
            _randomSource = randomSource;
            _spawningPositions = spawningPositions;
            _zombieControl = zombieControl;
            _noiseManager = noiseManager;
        }
        
        public NoiseResponderControl Create()
        {
            return new NoiseResponderControl(
                _randomSource,
                _spawningPositions,
                _zombieControl,
                _noiseManager.Noise);
        }
    }
    
    public class NoiseResponderControl
    {
        private readonly RandomSource _randomSource;
        private readonly SpawningPositions _spawningPositions;
        private readonly ZombieControl _zombieControl;
        private readonly IObservable<NoiseEvent> _noises;
        private readonly Subject<Unit> _occurred = new();
        private readonly Percent _chance = new Percent(0.3);

        public NoiseResponderControl(
            RandomSource randomSource,
            SpawningPositions spawningPositions,
            ZombieControl zombieControl,
            IObservable<NoiseEvent> noises)
        {
            _randomSource = randomSource;
            _spawningPositions = spawningPositions;
            _zombieControl = zombieControl;
            _noises = noises;
        }
        
        public IDisposable ApplyTo(ZombieGroup group, out IObservable<Unit> occurred)
        {
            occurred = _occurred;
            return _noises.Subscribe(noise =>
            {
                Logger.Info("Sending zombie towards player");
                if (_randomSource.NextChance(_chance))
                {
                    _zombieControl.SendGroupTowards(group, noise.Origin);
                    _occurred.OnNext(Unit.Default);
                }
            });
        }
    }
}