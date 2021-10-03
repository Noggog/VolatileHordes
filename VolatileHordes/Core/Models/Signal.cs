using System;
using System.Reactive;
using System.Reactive.Subjects;

namespace VolatileHordes.Utility
{
    public class Signal
    {
        private Subject<Unit> _redirect = new();
        public IObservable<Unit> Signalled => _redirect;

        public void Fire()
        {
            _redirect.OnNext(Unit.Default);
        }
    }
}