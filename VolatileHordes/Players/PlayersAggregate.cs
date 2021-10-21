using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Players
{
    public class PlayersAggregate
    {
        public IEnumerable<Player> players { get; }
        public PlayersAggregate(IEnumerable<Player> players)
        {
            this.players = players;
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
