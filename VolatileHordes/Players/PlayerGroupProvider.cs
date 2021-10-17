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
        public List<PlayerParty> playerParties = new();
        public PlayerGroupProvider(
            PlayerGameEventsWrapper playerGameEventsWrapper,
            IWorld world,
            GameStageCalculator gameStageCalculator
        )
        {
            playerParties.Add(new(gameStageCalculator));
            var playerIDs =
                Observable.Merge(
                    playerGameEventsWrapper.PlayerAdded.Select(x => new ValueTuple<bool, int>(true, x)),
                    playerGameEventsWrapper.PlayerRemoved.Select(x => new ValueTuple<bool, int>(false, x))
                )
                .Subscribe(x =>
                {
                    if (x.Item1)
                        playerParties[0].players.Add(x.Item2, new Player(world, x.Item2));
                    else
                        playerParties[0].players.Remove(x.Item2);
                });
        }
    }
}
