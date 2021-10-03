using System;

namespace VolatileHordes.Utility
{
    public static class DisposableExt
    {
        public static void DisposeWith(this IDisposable disp, IDisposableBucket bucket)
        {
            bucket.AddForDisposal(disp);
        }
    }
}