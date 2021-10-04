using System.Collections.Generic;
using VolatileHordes.Director;

namespace VolatileHordes.Players
{
    public class PlayerGroup
    {
        private readonly GameStageCalculator _gameStageCalculator;
        public List<PlayerZone> Players = new();

        public float GameStage => _gameStageCalculator.GetGamestage(this);

        public PlayerGroup(GameStageCalculator gameStageCalculator)
        {
            _gameStageCalculator = gameStageCalculator;
        }
    }
}