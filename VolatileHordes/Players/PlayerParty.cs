using System.Collections.Generic;
using VolatileHordes.Allocation;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Players
{
    public class PlayerParty
    {
        private readonly GameStageCalculator _gameStageCalculator;
        public Dictionary<int, IPlayer> players = new();

        public float GameStage => _gameStageCalculator.GetGamestage(this);

        public PlayerParty(GameStageCalculator gameStageCalculator)
        {
            _gameStageCalculator = gameStageCalculator;
        }
    }
}