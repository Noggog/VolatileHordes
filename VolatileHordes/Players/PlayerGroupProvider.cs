using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Utility;

namespace VolatileHordes.Players
{
    public class PlayerGroupProvider
    {
        public IObservable<IEnumerable<Player>> players;
        public IObservable<IEnumerable<PlayerZone>> playerZones;
        public IObservable<IEnumerable<PlayerGroup>> playerGroups;
        public PlayerGroupProvider(
            PlayerGameEventsWrapper playerGameEventsWrapper,
            IWorld world,
            GameStageCalculator gameStageCalculator,
            TimeManager timeManager
        )
        {
            var playerIDs =
                Observable.Merge(
                    playerGameEventsWrapper.PlayerAdded.Select(x => new ValueTuple<bool, int>(true, x)),
                    playerGameEventsWrapper.PlayerRemoved.Select(x => new ValueTuple<bool, int>(false, x))
                )
                .Scan(new List<int>(), (acc, x) =>
                {
                    if (x.Item1)
                        acc.Add(x.Item2);
                    else
                        acc.Remove(x.Item2);
                    return acc;
                })
                .Also(x => x.Subscribe(x => Logger.Temp("playerIDs:{0}", x)));
            players = playerIDs.Select(x => x.Select(x => new Player(world, x)))
                .Also(x => x.Subscribe(x => Logger.Temp("players:{0}", x)));
            playerZones = players.Select(x => x.Select(x => new PlayerZone(x)))
                .Also(x => x.Subscribe(x => Logger.Temp("playerZones:{0}", x)));

            /**
             * Recursively collects groups of players that are close together.
             */
            List<PlayerGroup> GroupPlayersTogether(IEnumerable<PlayerZone> playerZones)
            {
                var playerZoneGroups = new List<List<PlayerZone>>();
                void ExtractMatches(PlayerZone playerZoneI)
                {
                    playerZoneGroups.Last().Add(playerZoneI);
                    foreach (var playerZoneJ in playerZones)
                    {
                        if (playerZoneGroups.SelectMany(x => x).Contains(playerZoneJ))
                            continue;
                        if (playerZoneI.SpawnRectangle.IntersectsWith(playerZoneJ.SpawnRectangle))
                            ExtractMatches(playerZoneJ);
                    }
                }
                foreach (var playerZoneI in playerZones)
                {
                    if (playerZoneGroups.SelectMany(x => x).Contains(playerZoneI))
                        continue;
                    playerZoneGroups.Add(new List<PlayerZone>());
                    ExtractMatches(playerZoneI);
                }
                return playerZoneGroups.Select(x => new PlayerGroup(gameStageCalculator, x.Select(y => y.Player).ToList())).ToList();
            }

            playerGroups =
                Observable.CombineLatest(
                    playerZones,
                    timeManager.Interval(TimeSpan.FromSeconds(30)),
                    (playerZones, _) => GroupPlayersTogether(playerZones)
                )
                .Also(x => x.Subscribe(x => Logger.Temp("playerGroups:{0}", x)));
        }
    }
}
