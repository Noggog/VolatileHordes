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

        private readonly BehaviorSubject<DateTime> _updateTime;

        private readonly IObservable<TimeSpan> _updateDeltas;

        public TimeManager(INowProvider nowProvider, RandomSource randomSource)
        {
            _nowProvider = nowProvider;
            _randomSource = randomSource;
            _updateTime = new BehaviorSubject<DateTime>(_nowProvider.Now);
            _updateDeltas = _updateTime
                .StartWith(DateTime.Now)
                .Pairwise()
                .Skip(1)
                .Select(x => x.Item2 - x.Item1)
                .Publish()
                .RefCount();
        }

        public void Update()
        {
            _updateTime.OnNext(_nowProvider.Now);
        }

        public IObservable<Unit> UpdateTicks() => _updateTime.Unit();

        public IObservable<Unit> Interval(TimeSpan timeSpan)
        {
            return _updateDeltas
                .Scan(
                    new ValueTuple<TimeSpan, bool>(new TimeSpan(), false),
                    (accum, delta) =>
                    {
                        var totalTimePassed = accum.Item1 + delta;
                        if (totalTimePassed < timeSpan)
                        {
                            return new ValueTuple<TimeSpan, bool>(totalTimePassed, false);
                        }
                        return new ValueTuple<TimeSpan, bool>(new TimeSpan() + totalTimePassed - timeSpan, true);
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