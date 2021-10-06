using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace VolatileHordes
{
    public static class TaskExt
    {
        public static async void FireAndForget(this Task task, Action<Exception>? onException = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
                when (onException != null)
            {
                onException(ex);
            }
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Func<Task> action)
        {
            return source
                .Select(_ => Observable.FromAsync(action))
                .Concat()
                .Subscribe();
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Func<T, Task> action)
        {
            return source
                .Select(l => Observable.FromAsync(() => action(l)))
                .Concat()
                .Subscribe();
        }

        public static IObservable<Unit> DoAsync<T>(this IObservable<T> source, Func<T, Task> task)
        {
            return source
                .Select(x => Observable.FromAsync(() => task(x)))
                .Concat();
        }

        public static IObservable<Unit> DoAsync<T>(this IObservable<T> source, Func<Task> task)
        {
            return source
                .Select(_ => Observable.FromAsync(task))
                .Concat();
        }

        public static IObservable<R> SelectAsync<T, R>(this IObservable<T> source, Func<Task<R>> task)
        {
            return source
                .Select(_ => Observable.FromAsync(task))
                .Concat();
        }

        public static IObservable<R> SelectAsync<T, R>(this IObservable<T> source, Func<T, Task<R>> task)
        {
            return source
                .Select(x => Observable.FromAsync(() => task(x)))
                .Concat();
        }
    }
}