using System.Collections.Generic;
using VolatileHordes.Director;

namespace VolatileHordes.Players
{
    public class PlayerPartiesProvider
    {
        public List<PlayerParty> playerParties = new();
        public PlayerPartiesProvider(
            GameStageCalculator gameStageCalculator,
            PlayersProvider playersProvider)
        {
            playerParties.Add(new(gameStageCalculator, playersProvider.players));
        }
    }
}
