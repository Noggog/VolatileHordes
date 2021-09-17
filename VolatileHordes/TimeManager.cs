using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using VolatileHordes.Randomization;

namespace VolatileHordes
{
    public class TimeManager
    {
        private readonly RandomSource _randomSource;
        
        private BehaviorSubject<DateTime> UpdateTime = new(DateTime.Now);

        public TimeManager(RandomSource randomSource)
        {
            _randomSource = randomSource;
        }

        public void Update()
        {
            UpdateTime.OnNext(DateTime.Now);
        }

        public IObservable<Unit> UpdateTicks() => UpdateTime.Unit();

        public IObservable<Unit> Interval(TimeSpan timeSpan)
        {
            return UpdateTime
                .Scan(
                    new ValueTuple<DateTime, bool>(DateTime.Now, false),
                    (accum, newItem) =>
                    {
                        if (newItem - accum.Item1 < timeSpan)
                        {
                            return new ValueTuple<DateTime, bool>(accum.Item1, false);
                        }
                        return new ValueTuple<DateTime, bool>(newItem, true);
                    })
                .Where(x => x.Item2)
                .Unit();
        }

        public IObservable<Unit> Timer(TimeSpan timeSpan)
        {
            return Interval(timeSpan)
                .Take(1);
        }

        public IObservable<Unit> IntervalWithVariance(TimeSpan timeSpan, double positivePercent)
        {
            return Observable
                .Create<IObservable<Unit>>(async (obs, cancel) =>
                {
                    while (!cancel.IsCancellationRequested)
                    {
                        var deviationPct = _randomSource.Random.NextDouble() * positivePercent;
                        var deviation = (int)(timeSpan.Ticks * deviationPct);
                        var spanToUse = timeSpan.Add(TimeSpan.FromTicks(deviation));
                        obs.OnNext(Timer(spanToUse));
                    }
                    return Disposable.Empty;
                })
                .Switch();
        }
    }
}