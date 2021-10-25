using System;
using System.Drawing;
using UniLinq;
using UnityEngine;
using VolatileHordes.Core.Services;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Players;
using VolatileHordes.Probability;
using VolatileHordes.Utility;

namespace VolatileHordes.Spawning
{
    public class SpawningPositions
    {
        private readonly IWorld _world;
        private readonly PlayersProvider playersProvider;
        private readonly RandomSource _randomSource;

        public SpawningPositions(
            IWorld world,
            PlayersProvider playersProvider,
            RandomSource randomSource)
        {
            _world = world;
            this.playersProvider = playersProvider;
            _randomSource = randomSource;
        }

        public SpawnTarget? GetRandomTarget(bool nearPlayer)
        {
            if (nearPlayer)
            {
                return GetRandomNearPlayer();
            }
            else
            {
                return GetRandomTarget();
            }
        }

        public SpawnTarget? GetRandomTarget()
        {
            var player = GetRandomPlayer();
            if (player == null) return null;
            var pos = playersProvider.playerParties.Find(x => x.players.Contains(player)).GetRandomSafeCorner();
            if (pos == null) return null;
            return new SpawnTarget(pos.Value, player);
        }

        public SpawnTarget? GetRandomNearPlayer()
        {
            var spawnTarget = GetRandomTarget();
            if (spawnTarget == null) return null;
            var newTarget = GetRandomEdgeRangeAwayFrom(spawnTarget.TriggerOrigin, range: 30);
            if (newTarget == null) return null;
            return new SpawnTarget(
                newTarget.Value,
                spawnTarget.Player);
        }

        public Player? GetRandomPlayer()
        {
            var players = playersProvider.players.ToArray();
            if (players.Length == 0)
                return default;

            for (int i = 0; i < 4; i++)
            {
                var idx = _randomSource.Get(0, players.Length);
                return players[idx];
            }

            return default;
        }

        public Vector3? GetRandomPointNear(PointF pt, byte range, int attemptCount = 10)
        {
            var rect = new RectangleF(
                x: pt.X - range,
                y: pt.Y - range,
                width: range * 2,
                height: range * 2);
            return GetRandomPosition(rect, attemptCount);
        }

        public Vector3? GetRandomEdgeRangeAwayFrom(PointF pt, byte range)
        {
            var rect = new RectangleF(
                x: pt.X - range,
                y: pt.Y - range,
                width: range * 2,
                height: range * 2);
            
            foreach (var corner in rect.Corners()
                .ToArray()
                .EnumerateFromRandomIndex(_randomSource))
            {
                var worldPos = _world.GetWorldVector(corner);
                if (_world.CanSpawnAt(worldPos))
                {
                    return worldPos;
                }
            }

            return null;
        }
        
        public Vector3? GetRandomPosition(RectangleF zone, int attemptCount = 10)
        {
            for (int i = 0; i < attemptCount; i++)
            {
                var pos = TryGetSingleRandomZonePos(zone);
                var worldPos = _world.GetWorldVector(pos);
                if (_world.CanSpawnAt(worldPos))
                {
                    return worldPos;
                }
            }

            return null;
        }
        
        public PointF TryGetSingleRandomZonePos(RectangleF zone)
        {
            return new PointF(
                _randomSource.Get(zone.Left, zone.Right),
                _randomSource.Get(zone.Bottom, zone.Top));
        }

        public Player? GetNearestPlayer(PointF pt)
        {
            return playersProvider.players
                .OrderBy(x => pt.AbsDistance(x.location))
                .FirstOrDefault();
        }
    }
}