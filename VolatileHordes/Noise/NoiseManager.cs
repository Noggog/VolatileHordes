using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using UnityEngine;
using VolatileHordes.Settings.User;
using VolatileHordes.Utility;

namespace VolatileHordes.Noise
{
    public interface INoiseSource
    {
        IObservable<NoiseEvent> Noise { get; }
    }

    public class NoiseManager : INoiseSource
    {
        private readonly TimeManager _timeManager;
        private readonly Dictionary<string, byte> _noisyNames = new(StringComparer.OrdinalIgnoreCase);

        private readonly Subject<NoiseEvent> _noiseEvents = new();

        public IObservable<NoiseEvent> Noise => _noiseEvents;

        public NoiseManager(
            TimeManager timeManager,
            NoiseSettings settings)
        {
            _timeManager = timeManager;
            foreach (var noise in settings.Noises)
            {
                _noisyNames[noise.Name] = noise.Volume;
            }
        }

        public void Notify(Vector3 position, string clipName, float volumeScale)
        {
            if (!_noisyNames.TryGetValue(clipName, out var volume)) return;
            var level = 1.0f * volume / 255;
            var ev = new NoiseEvent(position.ToPoint(), Percent.FactoryPutInRange(level * volumeScale));
            Logger.Verbose("Firing noise event of volume {0} at {1}", ev.Volume, ev.Origin);
            _noiseEvents.OnNext(ev);
        }
    }
}