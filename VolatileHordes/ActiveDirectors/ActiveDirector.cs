using System;
using System.Collections.Generic;
using UniLinq;
using VolatileHordes.Zones;

namespace VolatileHordes.ActiveDirectors
{
    public class ActiveDirector
    {
        private readonly PlayerZoneManager _playerZoneManager;
        private readonly List<ZombieGroup> _groups = new();
        private static readonly TimeSpan StaleGroupTime = TimeSpan.FromMinutes(15);

        public IReadOnlyList<ZombieGroup> Groups => _groups;

        public bool Paused => !_playerZoneManager.HasPlayers();

        public ActiveDirector(
            TimeManager timeManager,
            PlayerZoneManager playerZoneManager)
        {
            _playerZoneManager = playerZoneManager;
            timeManager.Interval(TimeSpan.FromSeconds(30))
                .Subscribe(CleanGroups);
        }

        public ZombieGroup NewGroup()
        {
            var zombieGroup = new ZombieGroup();
            _groups.Add(zombieGroup);
            return zombieGroup;
        }

        private void CleanGroups()
        {
            if (Paused) return;
            var now = DateTime.Now;
            for (int i = _groups.Count - 1; i >= 0; i--)
            {
                var g = _groups[i];
                if (now - g.SpawnTime < StaleGroupTime) continue;
                var count = g.Zombies
                    .Select(z => z.GetEntity())
                    .NotNull()
                    .Count(e => !e.IsDead());
                if (count <= 1)
                {
                    Logger.Info("Cleaning group of {0} zombies with only {1} active.", g.Zombies.Count, count);
                    var group = _groups[i];
                    _groups.RemoveAt(i);
                    group.Dispose();
                }
            }
        }
    }
}