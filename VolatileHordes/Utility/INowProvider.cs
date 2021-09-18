using System;

namespace VolatileHordes.Utility
{
    public interface INowProvider
    {
        DateTime Now { get; }
    }

    public class NowProvider : INowProvider
    {
        public DateTime Now => DateTime.Now;
    }
}