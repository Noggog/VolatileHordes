using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using VolatileHordes.Players;
using VolatileHordes.Probability;
using VolatileHordes.Utility;

namespace VolatileHordes
{
    public class TimeManager
    {
        private readonly INowProvider _nowProvider;
        private readonly PlayerCountProvider playerCountProvider;
        private readonly RandomSource _randomSource;

        private readonly BehaviorSubject<DateTime> _updateTime;

        public IObservable<TimeSpan> UpdateDeltas { get; }

        public TimeManager(
            INowProvider nowProvider,
            PlayerCountProvider playerCountProvider,
            RandomSource randomSource)
        {
            this.playerCountProvider = playerCountProvider;
            _nowProvider = nowProvider;
            _randomSource = randomSource;
            _updateTime = new BehaviorSubject<DateTime>(_nowProvider.Now);
            UpdateDeltas = _updateTime
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

        public IObservable<Unit> Interval(TimeSpan timeSpan, bool pauseIfNoPlayers = true)
        {
            var source = UpdateDeltas;
            if (pauseIfNoPlayers)
            {
                source =
                    Observable.CombineLatest(
                        UpdateDeltas,
                        playerCountProvider.playerCount,
                        (delta, numPlayers) => numPlayers > 0 ? delta : new TimeSpan()
                    )
                        .Where(x => x.Ticks > 0);
            }

            return source
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

        public IObservable<Unit> Timer(TimeSpan timeSpan, bool pauseIfNoPlayers = true)
        {
            return Interval(timeSpan, pauseIfNoPlayers)
                .Take(1);
        }

        public IObservable<Unit> IntervalWithVariance(TimeRange timeRange, bool pauseIfNoPlayers = true)
        {
            return Observable.Defer(() => Observable.Return(_randomSource.GetRandomTime(timeRange)))
                .Select(x => Timer(x, pauseIfNoPlayers))
                .Concat()
                .Take(1)
                .Repeat();
        }

        public IObservable<Unit> IntervalWithVariance(TimeRange timeRange, Action<TimeSpan> onNewInterval, bool pauseIfNoPlayers = true)
        {
            return Observable.Defer(() => Observable.Return(_randomSource.GetRandomTime(timeRange)))
                .Select(timeSpan =>
                {
                    onNewInterval(timeSpan);
                    return Timer(timeSpan, pauseIfNoPlayers);
                })
                .Concat()
                .Take(1)
                .Repeat();
        }

        public async Task Delay(TimeSpan span, bool pauseIfNoPlayers = true)
        {
            await Timer(span, pauseIfNoPlayers);
        }
    }
}