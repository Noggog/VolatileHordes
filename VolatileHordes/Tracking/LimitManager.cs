using System;
using System.Threading.Tasks;
using UniLinq;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Spawning;
using VolatileHordes.Utility;

namespace VolatileHordes.Tracking
{
    public class LimitManager
    {
        private readonly IWorld _world;
        private readonly ZombieGroupManager _groupManager;
        private static readonly int GameMaximumZombies = GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies);
        private static readonly int DesiredMaximumZombies = (int)(GameMaximumZombies * 0.5);

        public int CurrentlyActiveZombies => GameStats.GetInt(EnumGameStats.EnemyCount);

        public LimitManager(
            IWorld world,
            ZombieGroupManager groupManager)
        {
            _world = world;
            _groupManager = groupManager;
        }

        public void PrintZombieStats()
        {
            Logger.Info("Currently {0} zombies. {1}% of total. {2}% of allowed", CurrentlyActiveZombies, (100.0f * CurrentlyActiveZombies / GameMaximumZombies), (100.0f * CurrentlyActiveZombies / DesiredMaximumZombies));
        }

        public async Task CheckLimit()
        {
            var above = CurrentlyActiveZombies - DesiredMaximumZombies;
            if (above > 0)
            {
                await CreateRoomFor(checked((ushort)above));
            }
        }

        public ushort GetAllowedLimit(ushort desired)
        {
            var amount = Math.Min(desired, DesiredMaximumZombies - CurrentlyActiveZombies);
            var ret = checked((ushort)amount);
            if (ret != desired)
            {
                Logger.Debug("Was {0} active zombies with a desired max of {1}.  Reduced the number of zombies to be spawned from {2} to {3}", CurrentlyActiveZombies, DesiredMaximumZombies, desired, ret);
            }

            return ret;
        }

        public async Task CreateRoomFor(ushort count)
        {
            var above = CurrentlyActiveZombies + count - DesiredMaximumZombies;
            if (above > 0)
            {
                Logger.Debug("Was {0} active zombies and want to add {1} with a desired max of {2}.  Deleting {3} to make room", CurrentlyActiveZombies, count, DesiredMaximumZombies, above);
                await Destroy(checked((ushort)above));
            }
        }

        public async Task Destroy(ushort count)
        {
            Logger.Debug("Destroying farthest {0} zombies", count);
            foreach (var zombie in _groupManager.AllGroups
                .SelectMany(group => group.Zombies)
                .OrderByDescending(zombie =>
                {
                    if (zombie.Destroyed) return default(float?);
                    var pos = zombie.GetPosition();
                    if (pos == null) return default(float?);
                    var player = _world.GetClosestPlayer(pos.Value);
                    if (player == null) return default(float?);
                    var diff = _world.GetWorldVector(pos.Value) - player.GetPosition();
                    return diff.magnitude;
                })
                .NotNull()
                .Take(count))
            {
                Logger.Debug("Destroying {0}", zombie);
                zombie.Destroy();
            }

            await Task.Delay(100);
        }
    }
}