using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using VolatileHordes.Core.ObservableTransforms;

namespace VolatileHordes
{
    public static class ObservableExt
    {
        public static IObservable<TResult> SwitchMap<TSource, TResult>(this IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector)
        {
            return source.Select(selector).Switch();
        }

        public static IObservable<Unit> Unit<T>(this IObservable<T> source)
        {
            return source.Select(_ => System.Reactive.Unit.Default);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> obs, Action sub, Action<Exception>? onError = null)
        {
            return obs.Subscribe(_ => sub(),
                onError: onError);
        }

        public static IObservable<ValueTuple<T?, T>> Pairwise<T>(this IObservable<T> source)
        {
            T? prevStorage = default;
            return source.Select(i =>
            {
                var prev = prevStorage;
                prevStorage = i;
                return new ValueTuple<T?, T>(prev, i);
            });
        }

        public static IObservable<T> DisposeOld<T>(this IObservable<T> source)
            where T : IDisposable
        {
            return source.Pairwise()
                .Do(x => x.Item1?.Dispose())
                .Select(x => x.Item2);
        }

        public static IDisposable SubscribeAsync<T>(
            this IObservable<T> source,
            Func<Task> action, 
            Action<Exception>? onError = null)
        {
            return source
                .Select(_ => Observable.FromAsync(action))
                .Concat()
                .Subscribe(
                    onNext: _ => { },
                    onError: onError ?? (_ => { }));
        }

        public static IDisposable SubscribeAsync<T>(this IObservable<T> source, 
            Func<T, Task> action, 
            Action<Exception>? onError = null)
        {
            return source
                .Select(l => Observable.FromAsync(() => action(l)))
                .Concat()
                .Subscribe(
                    onNext: _ => { },
                    onError: onError ?? (_ => { }));
        }

        public static IObservable<bool> TimedSignalFlowShutoff(this IObservable<Unit> obs, TimeSpan timeToTurnOff, TimeManager timeManager)
        {
            return Observable.Merge(
                    obs.Select(x => false),
                    timeManager.UpdateDeltas.Select(x => (TimeSpan?)x)
                        .Merge(obs.Select(x => default(TimeSpan?)))
                        .Scan(TimeSpan.Zero, (seed, val) =>
                        {
                            if (val == null) return TimeSpan.Zero;
                            return seed + val.Value;
                        })
                        .Where(x => x > timeToTurnOff)
                        .Select(x => true))
                .DistinctUntilChanged();
        }

        public static IObservable<T> FlowSwitch<T>(this IObservable<T> source, IObservable<bool> flowSwitch)
        {
            return flowSwitch
                .Select(on =>
                {
                    if (on)
                    {
                        return source;
                    }
                    else
                    {
                        return Observable.Empty<T>();
                    }
                })
                .Switch();
        }

        public static IObservable<TOut> Compose<TIn, TOut>(this IObservable<TIn> source, 
            IObservableTransformation<TIn, TOut> transformation)
        {
            return transformation.Transform(source);
        }

        public static IObservable<TOut> Compose<TIn, TOut, T1>(this IObservable<TIn> source, 
            IObservableTransformation<TIn, TOut, T1> transformation, 
            T1 param1)
        {
            return transformation.Transform(source, param1);
        }

        public static IObservable<TOut> Compose<TIn, TOut, T1, T2>(this IObservable<TIn> source, 
            IObservableTransformation<TIn, TOut, T1, T2> transformation,
            T1 param1, T2 param2)
        {
            return transformation.Transform(source, param1, param2);
        }

        public static IObservable<TOut> Compose<TIn, TOut, T1, T2, T3>(this IObservable<TIn> source, 
            IObservableTransformation<TIn, TOut, T1, T2, T3> transformation,
            T1 param1, T2 param2, T3 param3)
        {
            return transformation.Transform(source, param1, param2, param3);
        }

        public static IObservable<TOut> Compose<TIn, TOut, T1, T2, T3, T4>(this IObservable<TIn> source, 
            IObservableTransformation<TIn, TOut, T1, T2, T3, T4> transformation,
            T1 param1, T2 param2, T3 param3, T4 param4)
        {
            return transformation.Transform(source, param1, param2, param3, param4);
        }
    }
}