using System;
using System.Reactive;
using System.Reactive.Linq;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Players
{
    public class PlayerZoneProvider
    {
        public IObservable<PlayerZone> playerZone;
        public PlayerZoneProvider(
            TimeManager timeManager,
            IPlayer player)
        {
            /*
             * [playerZoneObservable] creates and caches a new PlayerZone every 5s.
             * The cache is forgotten if, after the initial 4.5s, there are no observers.
             */
            playerZone =
                Observable.Merge(
                    Observable.Return(Unit.Default)
                        // Remember the cache for at least 4.5s
                        .Do(_ => playerZone.TakeUntil(timeManager.Timer(TimeSpan.FromSeconds(4.5))).Subscribe()),
                    timeManager.Interval(TimeSpan.FromSeconds(5))
                )
                    .Select(_ => new PlayerZone(player))
                    .Replay(1).RefCount();
        }
    }
}
