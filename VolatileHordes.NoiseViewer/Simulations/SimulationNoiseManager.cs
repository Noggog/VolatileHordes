using System;
using System.Drawing;
using System.Reactive.Subjects;
using VolatileHordes.Noise;
using VolatileHordes.Utility;

namespace VolatileHordes.NoiseViewer.Simulations
{
    public class SimulationNoiseManager : INoiseSource
    {
        private Subject<NoiseEvent> _noise = new();

        public void CauseNoise()
        {
            _noise.OnNext(new NoiseEvent(new PointF(0, 0), Percent.One));
        }
        
        public IObservable<NoiseEvent> Noise => _noise;
    }
}