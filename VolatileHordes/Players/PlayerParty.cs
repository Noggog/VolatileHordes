using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using VolatileHordes.Core.Services;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Probability;

namespace VolatileHordes.Players
{
    public class PlayerParty
    {
        public Dictionary<int, Player> playersDictionary = new();
        public IEnumerable<Player> players { get => playersDictionary.Values; }
        public float GameStage => gameStageCalculator.GetGamestage(this);

        private RandomSource randomSource;
        private IWorld world;
        private readonly GameStageCalculator gameStageCalculator;

        public PlayerParty(
            RandomSource randomSource,
            GameStageCalculator gameStageCalculator,
            IWorld world
        )
        {
            this.randomSource = randomSource;
            this.gameStageCalculator = gameStageCalculator;
            this.world = world;
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

        public Vector3? GetRandomSafeCorner()
        {
            var corners = ZoneProcessing.EdgeCornersFromCluster(
                    GetConnectedSpawnRects()
                        .ToArray()
                )
                .ToArray();

            foreach (var corner in corners.EnumerateFromRandomIndex(randomSource))
            {
                var worldPos = world.GetWorldVector(corner);
                if (world.CanSpawnAt(worldPos))
                {
                    return worldPos;
                }
            }

            return null;
        }
    }
}