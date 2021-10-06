using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolatileHordes.Director
{
    public class BasicSpawnDirector
    {
        private readonly TimeManager timeManager;

        public BasicSpawnDirector(TimeManager timeManager)
        {
            this.timeManager = timeManager;
        }

        public void start()
        {
            Logger.Temp("bbb");
            timeManager.IntervalWithVariance(
                new TimeRange(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(10)),
                onNewInterval: timeSpan => Logger.Temp("Will emit in {0}", timeSpan)
            )
                .Subscribe(x =>
                {
                    Logger.Temp("qwer");
                });
        }
    }
}
