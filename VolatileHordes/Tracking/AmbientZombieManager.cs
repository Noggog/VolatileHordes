using VolatileHordes.AiPackages;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Tracking
{
    public class AmbientZombieManager
    {
        private readonly IWorld _world;
        private readonly GroupManager _groupManager;

        private readonly ZombieGroup _group;

        public AmbientZombieManager(
            IWorld world,
            GroupManager groupManager,
            AmbientAiPackage aiPackage)
        {
            _world = world;
            _groupManager = groupManager;
            _group = new(aiPackage);
            aiPackage.ApplyTo(_group);
        }
        
        public void ZombieSpawned(int entityId)
        {
            var zombie = new Zombie(_world, entityId);
            if (_groupManager.ContainsZombie(zombie)) return;
            Logger.Verbose("Ambiently tracking zombie {0}", entityId);
            _group.Add(zombie);
        }

        public void ZombieDespawned(int entityId)
        {
            var zombie = new Zombie(_world, entityId);
            if (_group.Remove(zombie))
            {
                Logger.Verbose("Untracking ambient zombie {0}", entityId);
            }
        }

        public void MarkTracked(IZombie zombie)
        {
            if (_group.Remove(zombie))
            {
                Logger.Verbose("Untracking zombie because it was claimed by another group {0}", zombie.Id);
            }
        }
    }
}