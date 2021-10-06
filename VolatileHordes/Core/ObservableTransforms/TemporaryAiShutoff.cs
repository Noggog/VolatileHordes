using System;
using System.Reactive;
using System.Reactive.Linq;

namespace VolatileHordes.Core.ObservableTransforms
{
    public class TemporaryAiShutoff : IObservableTransformation<Unit, Unit, IObservable<Unit>, TimeSpan, string, string>
    {
        private readonly TimedSignalFlowShutoff _timedSignalFlowShutoff;

        public TemporaryAiShutoff(TimedSignalFlowShutoff timedSignalFlowShutoff)
        {
            _timedSignalFlowShutoff = timedSignalFlowShutoff;
        }
        
        public IObservable<Unit> Transform(IObservable<Unit> obs, IObservable<Unit> signal, TimeSpan timeToTurnOff, string componentName, string reason)
        {
            return obs.FlowSwitch(
                signal.Compose(_timedSignalFlowShutoff, timeToTurnOff)
                    .Do(flowing =>
                    {
                        if (flowing)
                        {
                            Logger.Debug("Turning {0} on", componentName);
                        }
                        else
                        {
                            Logger.Debug("Turning {0} off due to {1}", componentName, reason);
                        }
                    }));
        }
    }
}