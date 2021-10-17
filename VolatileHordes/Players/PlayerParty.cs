using System.Collections.Generic;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Players
{
    public class PlayerParty
    {
        private readonly GameStageCalculator _gameStageCalculator;
        public List<IPlayer> players = new();

        public float GameStage => _gameStageCalculator.GetGamestage(this);

        public PlayerParty(GameStageCalculator gameStageCalculator, List<IPlayer> players)
        {
            this.players = players;
            _gameStageCalculator = gameStageCalculator;
        }
    }
}