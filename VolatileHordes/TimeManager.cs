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
        
        public IObservable<Unit> IntervalWithVariance(TimeRange timeRange)
        {
            return Observable.Defer(() => Observable.Return(_randomSource.GetRandomTime(timeRange)))
                .Select(timeSpan =>
                {
                    return Timer(timeSpan);
                })
                .Concat()
                .Take(1)
                .Repeat();
        }
        
        public IObservable<Unit> IntervalWithVariance(TimeRange timeRange, Action<TimeSpan> onNewInterval)
        {
            return Observable.Defer(() => Observable.Return(_randomSource.GetRandomTime(timeRange)))
                .Select(timeSpan =>
                {
                    onNewInterval(timeSpan);
                    return Timer(timeSpan);
                })
                .Concat()
                .Take(1)
                .Repeat();
        }
    }
}