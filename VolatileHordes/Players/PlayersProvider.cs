using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Utility;

namespace VolatileHordes.Players
{
    public class PlayersProvider
    {
        public IObservable<IEnumerable<Player>> players;
        public PlayersProvider(
            PlayerGameEventsWrapper playerGameEventsWrapper,
            PlayerFactory playerFactory)
        {
            players =
                Observable.Merge(
                    playerGameEventsWrapper.PlayerAdded.Select(x => new ValueTuple<bool, int>(true, x)),
                    playerGameEventsWrapper.PlayerRemoved.Select(x => new ValueTuple<bool, int>(false, x))
                )
                .Scan(new Dictionary<int, Player>(), (acc, x) =>
                {
                    if (x.Item1)
                        acc.Add(x.Item2, playerFactory.create(x.Item2));
                    else
                        acc.Remove(x.Item2);
                    return acc;
                })
                .Select(x => x.Values)
                .Replay(1).RefCount()
                // # Stay up-to-date
                .Also(x => x.Subscribe());
        }
    }
}
