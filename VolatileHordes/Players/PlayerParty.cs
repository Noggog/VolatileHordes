using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public IEnumerable<RectangleF> GetConnectedSpawnRects()
        {
            return GetConnectedSpawnRects(players.First(), players.Skip(1), new HashSet<int>());
        }

        private IEnumerable<RectangleF> GetConnectedSpawnRects(Player playerI, IEnumerable<Player> players, HashSet<int> passedPlayers)
        {
            yield return playerI.SpawnRectangle;
            passedPlayers.Add(playerI.EntityId);

            foreach (var playerJ in players)
            {
                if (passedPlayers.Contains(playerJ.EntityId)) continue;
                if (!playerJ.SpawnRectangle.IntersectsWith(playerI.SpawnRectangle)) continue;

                foreach (var rhsRect in GetConnectedSpawnRects(playerJ, players, passedPlayers))
                {
                    yield return rhsRect;
                }
            }
        }
    }
}