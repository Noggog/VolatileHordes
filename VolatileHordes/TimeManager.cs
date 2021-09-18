using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using VolatileHordes.Randomization;
using VolatileHordes.Utility;

namespace VolatileHordes
{
    public class TimeManager
    {
        private readonly INowProvider _nowProvider;
        private readonly RandomSource _randomSource;

        private BehaviorSubject<DateTime> UpdateTime;

        public TimeManager(INowProvider nowProvider, RandomSource randomSource)
        {
            _nowProvider = nowProvider;
            _randomSource = randomSource;
            UpdateTime = new BehaviorSubject<DateTime>(_nowProvider.Now);
        }

        public void Update()
        {
            UpdateTime.OnNext(_nowProvider.Now);
        }

        public IObservable<Unit> UpdateTicks() => UpdateTime.Unit();

        public IObservable<Unit> Interval(TimeSpan timeSpan)
        {
            return UpdateTime
                .Scan(
                    new ValueTuple<DateTime, bool>(_nowProvider.Now, false),
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
                .Select(Timer)
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