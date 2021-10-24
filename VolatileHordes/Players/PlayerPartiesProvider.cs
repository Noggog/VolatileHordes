using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Probability;

namespace VolatileHordes.Players
{
    public class PlayerPartiesProvider
    {
        public List<PlayerParty> playerParties = new();
        public PlayerPartiesProvider(
            PlayerGameEventsWrapper playerGameEventsWrapper,
            IWorld world,
            GameStageCalculator gameStageCalculator,
            RandomSource randomSource
        )
        {
            playerParties.Add(new(randomSource, gameStageCalculator, world));
            Observable.Merge(
                playerGameEventsWrapper.PlayerAdded.Select(x => new ValueTuple<bool, int>(true, x)),
                playerGameEventsWrapper.PlayerRemoved.Select(x => new ValueTuple<bool, int>(false, x))
            )
            .Subscribe(x =>
            {
                if (x.Item1)
                    playerParties[0].playersDictionary.Add(x.Item2, new Player(world, x.Item2));
                else
                    playerParties[0].playersDictionary.Remove(x.Item2);
            });
        }
    }
}
