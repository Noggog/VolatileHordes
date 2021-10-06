using System;
using System.Reactive;
using System.Reactive.Linq;

namespace VolatileHordes.Core.ObservableTransforms
{
    public class TimedSignalFlowShutoff : IObservableTransformation<Unit, bool, TimeSpan>
    {
        private readonly TimeManager _timeManager;

        public TimedSignalFlowShutoff(TimeManager timeManager)
        {
            _timeManager = timeManager;
        }
        
        public IObservable<bool> Transform(IObservable<Unit> obs, TimeSpan timeToTurnOff)
        {
            return Observable.Merge(
                    obs.Select(_ => false),
                    _timeManager.UpdateDeltas.Select(x => (TimeSpan?)x)
                        .Merge(obs.Select(_ => default(TimeSpan?)))
                        .Scan(TimeSpan.Zero, (seed, val) =>
                        {
                            if (val == null) return TimeSpan.Zero;
                            return seed + val.Value;
                        })
                        .Where(x => x > timeToTurnOff)
                        .Select(_ => true))
                .StartWith(true)
                .DistinctUntilChanged();
        }
    }
}