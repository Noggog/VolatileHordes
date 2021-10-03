using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UnityEngine;
using VolatileHordes.Settings.User;
using VolatileHordes.Utility;

namespace VolatileHordes.Noise
{
    public interface INoiseSource
    {
        IObservable<NoiseEvent> Noise { get; }
        IObservable<Unit> NoiseReduction { get; }
    }

    public class NoiseManager : INoiseSource
    {
        private const int NoiseLostPulseSeconds = 2;
        private readonly TimeManager _timeManager;
        private readonly Dictionary<string, float> _noisyNames = new(StringComparer.OrdinalIgnoreCase);

        private readonly Subject<NoiseEvent> _noiseEvents = new();

        public IObservable<NoiseEvent> Noise => _noiseEvents;
        public IObservable<Unit> NoiseReduction { get; }

        public NoiseManager(
            TimeManager timeManager,
            NoiseSettings settings)
        {
            _timeManager = timeManager;
            foreach (var noise in settings.Noises)
            {
                _noisyNames[noise.Name] = noise.Volume;
            }

            NoiseReduction = timeManager.Interval(TimeSpan.FromSeconds(NoiseLostPulseSeconds))
                .Unit()
                .Publish().RefCount();
        }

        public void Notify(Vector3 position, string clipName, float volumeScale)
        {
            if (!_noisyNames.TryGetValue(clipName, out var volume)) return;
            var ev = new NoiseEvent(position.ToPoint(), Percent.FactoryPutInRange(volume / 100 * volumeScale));
            Logger.Verbose("Firing noise event of volume {0} at {1}, with original volume of {2} and scaled {3}", ev.Volume, ev.Origin, volume, volumeScale);
            _noiseEvents.OnNext(ev);
        }
    }
}