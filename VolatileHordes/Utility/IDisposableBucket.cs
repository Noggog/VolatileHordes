using System;

namespace VolatileHordes.Utility
{
    public interface IDisposableBucket
    {
        void AddForDisposal(IDisposable disp);
    }
}