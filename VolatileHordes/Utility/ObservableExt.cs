using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace VolatileHordes
{
    public static class ObservableExt
    {
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
                    onError: onError);
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
                    onError: onError);
        }
    }
}