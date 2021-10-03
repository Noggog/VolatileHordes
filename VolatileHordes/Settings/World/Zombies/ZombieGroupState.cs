using System.Drawing;
using UniLinq;
using VolatileHordes.AiPackages;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Tracking;

namespace VolatileHordes.Settings.World.Zombies
{
    public class ZombieGroupState
    {
        public ZombieState[] Zombies { get; set; } = new ZombieState[0];
        public PointF? Target { get; set; }
        public AiPackageEnum? AiPackage { get; set; }

        public void ApplyToWorld()
        {
            using var groupSpawn = Container.GroupManager.NewGroup(Container.AiPackageMapper.Get(AiPackage));
            groupSpawn.Group.Add(
                Zombies
                    .Select<ZombieState, IZombie>(z => new Zombie(Container.World, z.EntityId))
                    .ToArray());
            groupSpawn.Group.Target = Target;
        }

        public static ZombieGroupState? GetSettings(ZombieGroup g)
        {
            if (g.Count == 0) return null;
            return new ZombieGroupState()
            {
                Zombies = g
                    .Select(z => new ZombieState()
                    {
                        EntityId = z.Id
                    })
                    .ToArray(),
                Target = g.Target,
                AiPackage = g.AiPackage?.TypeEnum
            };
        }
    }
}