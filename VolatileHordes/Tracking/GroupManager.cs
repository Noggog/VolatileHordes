﻿using System;
using System.Collections.Generic;
using VolatileHordes.AiPackages;
using VolatileHordes.Spawning;
using VolatileHordes.Zones;

namespace VolatileHordes.Tracking
{
    public class GroupManager
    {
        private readonly PlayerZoneManager _playerZoneManager;
        private readonly List<ZombieGroup> _groups = new();
        private static readonly TimeSpan StaleGroupTime = TimeSpan.FromMinutes(15);

        public IReadOnlyList<ZombieGroup> Groups => _groups;

        public bool Paused => !_playerZoneManager.HasPlayers();

        public GroupManager(
            TimeManager timeManager,
            PlayerZoneManager playerZoneManager)
        {
            _playerZoneManager = playerZoneManager;
            timeManager.Interval(TimeSpan.FromSeconds(30))
                .Subscribe(CleanGroups);
        }

        public ZombieGroupSpawn NewGroup(IAiPackage? package = null)
        {
            var zombieGroup = new ZombieGroup(package);

            if (package != null)
            {
                Logger.Info("Applying AI {0} to group {1}", package.GetType().Name, zombieGroup.Id);
            }
            _groups.Add(zombieGroup);
            return new ZombieGroupSpawn(zombieGroup);
        }

        private void CleanGroups()
        {
            if (Paused) return;
            var now = DateTime.Now;
            for (int i = _groups.Count - 1; i >= 0; i--)
            {
                var g = _groups[i];
                if (now - g.SpawnTime < StaleGroupTime) continue;
                var count = g.NumAlive();
                if (count <= 1)
                {
                    Logger.Info("Cleaning {0}.", g);
                    var group = _groups[i];
                    _groups.RemoveAt(i);
                    group.Dispose();
                }
            }
        }

        public void DestroyAll()
        {
            foreach (var zombieGroup in _groups)
            {
                zombieGroup.Destroy();
                zombieGroup.Dispose();
            }
        }
    }
}