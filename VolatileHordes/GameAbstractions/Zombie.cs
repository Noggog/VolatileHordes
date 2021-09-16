using UnityEngine;

namespace VolatileHordes.GameAbstractions
{
    public interface IZombie
    {
        int Id { get; }
        bool SendTowards(Vector3 vector3);
        EntityZombie? GetEntity();
    }

    public class Zombie : IZombie
    {
        private readonly IWorld _world;
        public int Id { get; }

        public Zombie(
            IWorld world,
            int entityId)
        {
            _world = world;
            Id = entityId;
        }

        public EntityZombie? GetEntity() => _world.GetEntity(Id) as EntityZombie;

        public bool SendTowards(Vector3 vector3)
        {
            var entity = GetEntity();
            if (entity == null) return false;
            entity.SetInvestigatePosition(vector3, 6000, false);
            return true;
        }

        public override string ToString()
        {
            return GetEntity()?.ToString() ?? Id.ToString();
        }
    }
}