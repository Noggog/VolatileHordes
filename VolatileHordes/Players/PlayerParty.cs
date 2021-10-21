using System.Collections.Generic;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Players
{
    public class PlayerParty
    {
        private readonly GameStageCalculator _gameStageCalculator;
        public Dictionary<int, Player> playersDictionary = new();
        public IEnumerable<Player> players { get => playersDictionary.Values; }

        public float GameStage => _gameStageCalculator.GetGamestage(this);

        public PlayerParty(GameStageCalculator gameStageCalculator)
        {
            _gameStageCalculator = gameStageCalculator;
        }
    }
}