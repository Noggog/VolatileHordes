using System;
using System.Reactive;
using System.Reactive.Linq;

namespace MoreHordes
{
    public static class ObservableExt
    {
        public static IObservable<Unit> Unit<T>(this IObservable<T> source)
        {
            return source.Select(_ => System.Reactive.Unit.Default);
        }
    }
}