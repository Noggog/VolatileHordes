using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using UniLinq;
using VolatileHordes.AiPackages;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Players;
using VolatileHordes.Spawning;

namespace VolatileHordes.Tracking
{
    public class ZombieGroupManager
    {
        private readonly PlayerZoneManager _playerZoneManager;
        private readonly List<ZombieGroup> _normalGroups = new();
        private static readonly TimeSpan StaleGroupTime = TimeSpan.FromMinutes(15);

        public IReadOnlyList<ZombieGroup> NormalGroups => _normalGroups;
        public Dictionary<int, ZombieGroup> AmbientGroups { get; } = new();

        private Subject<ZombieGroup> _ambientGroupTracked = new();
        public IObservable<ZombieGroup> AmbientGroupTracked => _ambientGroupTracked;

        public IEnumerable<ZombieGroup> AllGroups => NormalGroups.Concat(AmbientGroups.Values);

        public bool Paused => !_playerZoneManager.HasPlayers();

        public ZombieGroupManager(
            TimeManager timeManager,
            PlayerZoneManager playerZoneManager)
        {
            _playerZoneManager = playerZoneManager;
            timeManager.Interval(TimeSpan.FromSeconds(30))
                .Subscribe(CleanGroups,
                    onError: e => Logger.Error("{0} had update error {1}", nameof(ZombieGroupManager), e));
        }

        public bool ContainsZombie(IZombie zombie)
        {
            foreach (var zombieGroup in _normalGroups)
            {
                if (zombieGroup.ContainsZombie(zombie))
                {
                    return true;
                }
            }

            return false;
        }

        public ZombieGroupSpawn NewGroup(IAiPackage? package = null)
        {
            var zombieGroup = new ZombieGroup(package);

            if (package != null)
            {
                Logger.Info("Applying AI {0} to group {1}", package.GetType().Name, zombieGroup.Id);
            }
            _normalGroups.Add(zombieGroup);
            return new ZombieGroupSpawn(zombieGroup);
        }

        public void CleanGroups()
        {
            if (Paused) return;
            
            var now = DateTime.Now;
            for (int i = _normalGroups.Count - 1; i >= 0; i--)
            {
                var g = _normalGroups[i];
                var count = g.NumNotDespawned();
                if (count == 0)
                {
                    Logger.Debug("Cleaning {0}.", g);
                    var group = _normalGroups[i];
                    _normalGroups.RemoveAt(i);
                    group.Dispose();
                }
            }

            List<int>? list = null;
            foreach (var group in AmbientGroups.Values)
            {
                var count = group.NumNotDespawned();
                if (count == 0)
                {
                    Logger.Debug("Cleaning {0}.", group);
                    if (list == null)
                    {
                        list = new();
                    }
                    list.Add(group.Id);
                    group.Dispose();
                }
            }

            if (list != null)
            {
                foreach (var id in list)
                {
                    AmbientGroups.Remove(id);
                }
            }
        }

        public void DestroyNormal()
        {
            foreach (var zombieGroup in _normalGroups)
            {
                zombieGroup.Destroy();
                zombieGroup.Dispose();
            }
            _normalGroups.Clear();
        }

        public void TrackAsAmbient(ZombieGroup group)
        {
            AmbientGroups[group.Id] = group;
            _ambientGroupTracked.OnNext(group);
        }
    }
}