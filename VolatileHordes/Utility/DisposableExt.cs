using System;
using System.Reactive.Disposables;

namespace VolatileHordes.Utility
{
    public static class DisposableExt
    {
        public static void DisposeWith(this IDisposable disp, IDisposableBucket bucket)
        {
            bucket.AddForDisposal(disp);
        }
        
        public static void DisposeWith(this IDisposable disp, CompositeDisposable bucket)
        {
            bucket.Add(disp);
        }
    }
}