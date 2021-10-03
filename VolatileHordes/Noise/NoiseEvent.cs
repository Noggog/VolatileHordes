using System.Drawing;
using VolatileHordes.Utility;

namespace VolatileHordes.Noise
{
    public struct NoiseEvent
    {
        public readonly PointF Origin;
        public readonly Percent Volume;

        public NoiseEvent(PointF origin, Percent volume)
        {
            Origin = origin;
            Volume = volume;
        }
    }
}