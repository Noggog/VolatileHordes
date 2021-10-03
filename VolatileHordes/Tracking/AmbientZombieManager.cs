using System.Collections.Generic;
using System.Drawing;
using UniLinq;
using VolatileHordes.AiPackages;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Tracking
{
    public class AmbientZombieManager
    {
        private readonly IWorld _world;
        private readonly GroupManager _groupManager;
        private readonly AmbientAiPackage _aiPackage;

        public Dictionary<int, ZombieGroup> Groups { get; } = new();

        public AmbientZombieManager(
            IWorld world,
            GroupManager groupManager,
            AmbientAiPackage aiPackage)
        {
            _world = world;
            _groupManager = groupManager;
            _aiPackage = aiPackage;
        }
        
        public void ZombieSpawned(int entityId)
        {
            var zombie = new Zombie(_world, entityId);
            if (_groupManager.ContainsZombie(zombie)) return;
            Logger.Verbose("Ambiently tracking zombie {0}", entityId);
            if (Groups.TryGetValue(entityId, out var group))
            {
                Logger.Warning("Zombie {0} already ambiently tracked", entityId);
            }
            group = new ZombieGroup(_aiPackage);
            _aiPackage.ApplyTo(group);
            group.Add(zombie);
            Groups[entityId] = group;
        }

        private bool TryRemove(int entityId)
        {
            if (Groups.TryGetValue(entityId, out var group))
            {
                group.Dispose();
                Groups.Remove(entityId);
                return true;
            }

            return false;
        }

        public void ZombieDespawned(int entityId)
        {
            if (TryRemove(entityId))
            {
                Logger.Verbose("Untracking ambient zombie {0}", entityId);
            }
        }

        public void MarkTracked(IZombie zombie)
        {
            if (TryRemove(zombie.Id))
            {
                Logger.Verbose("Untracking zombie because it was claimed by another group {0}", zombie.Id);
            }
        }

        public void DestroyAll()
        {
            foreach (var g in Groups.Values.ToArray())
            {
                g.Destroy();
            }
        }

        public void PrintRelativeTo(PointF pt)
        {
            foreach (var g in Groups.Values)
            {
                g.PrintRelativeTo(pt);
            }
        }
    }
}