using System.Drawing;
using UniLinq;
using VolatileHordes.AiPackages;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Tracking
{
    public class AmbientZombieManager
    {
        private readonly IWorld _world;
        private readonly ZombieGroupManager _groupManager;
        private readonly AmbientAiPackage _aiPackage;

        public bool AllowAmbient { get; set; } = true;

        public AmbientZombieManager(
            IWorld world,
            ZombieGroupManager groupManager,
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
            if (_groupManager.AmbientGroups.TryGetValue(entityId, out var group))
            {
                Logger.Warning("Zombie {0} already ambiently tracked", entityId);
                return;
            }

            if (zombie.IsSleeper)
            {
                Logger.Debug("Zombie {0} is a sleeper and won't be tracked", entityId);
                return;
            }

            if (!AllowAmbient)
            {
                zombie.Destroy();
                return;
            }
            
            group = new ZombieGroup(_aiPackage);
            _aiPackage.ApplyTo(group);
            group.Add(zombie);
            _groupManager.TrackAsAmbient(group);
        }

        private bool TryRemove(int entityId)
        {
            if (_groupManager.AmbientGroups.TryGetValue(entityId, out var group))
            {
                group.Dispose();
                _groupManager.AmbientGroups.Remove(entityId);
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
            foreach (var g in _groupManager.AmbientGroups.Values.ToArray())
            {
                g.Destroy();
            }
        }

        public void PrintRelativeTo(PointF pt)
        {
            foreach (var g in _groupManager.AmbientGroups.Values)
            {
                g.PrintRelativeTo(pt);
            }
        }
    }
}