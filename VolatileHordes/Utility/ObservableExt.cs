using System;
using System.Reactive;
using System.Reactive.Linq;

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
    }
}