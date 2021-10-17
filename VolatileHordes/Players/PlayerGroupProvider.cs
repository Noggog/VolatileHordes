using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;

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
            GameStageCalculator gameStageCalculator
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
                });
            players = playerIDs.Select(x => x.Select(x => new Player(world, x)));
            playerZones = players.Select(x => x.Select(x => new PlayerZone(x)));

            /**
             * Recursively collects groups of players that are close together.
             */
            List<PlayerGroup> GroupPlayersTogether(IEnumerable<PlayerZone> playerZones)
            {
                var playerZoneGroups = new List<List<PlayerZone>>();
                void extractMatches(PlayerZone playerZoneI)
                {
                    playerZoneGroups.Last().Add(playerZoneI);
                    foreach (var playerZoneJ in playerZones)
                    {
                        if (playerZoneGroups.SelectMany(x => x).Contains(playerZoneJ))
                            continue;
                        if (playerZoneI.SpawnRectangle.IntersectsWith(playerZoneJ.SpawnRectangle))
                            extractMatches(playerZoneJ);
                    }
                }
                foreach (var playerZoneI in playerZones)
                {
                    if (playerZoneGroups.SelectMany(x => x).Contains(playerZoneI))
                        continue;
                    playerZoneGroups.Add(new List<PlayerZone>());
                    extractMatches(playerZoneI);
                }
                return playerZoneGroups.Select(x => new PlayerGroup(gameStageCalculator, x.Select(y => y.Player).ToList())).ToList();
            }

            playerGroups = playerZones.Select(x => GroupPlayersTogether(x));
        }
    }
}
