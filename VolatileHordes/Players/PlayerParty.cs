using System;
using System.Collections.Generic;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Players
{
    public class PlayerParty
    {
        private readonly GameStageCalculator _gameStageCalculator;
        public IEnumerable<IPlayer> players = new List<IPlayer>();

        public float GameStage => _gameStageCalculator.GetGamestage(this);

        /**
         * [players] emits the list of players that belong to this party.
         */
        public PlayerParty(GameStageCalculator gameStageCalculator, IObservable<IEnumerable<IPlayer>> players)
        {
            _gameStageCalculator = gameStageCalculator;
            players.Subscribe(x => this.players = x);
        }
    }
}