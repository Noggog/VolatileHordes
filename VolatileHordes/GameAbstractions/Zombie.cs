using System.Drawing;
using UnityEngine;

namespace VolatileHordes.GameAbstractions
{
    public interface IZombie
    {
        int Id { get; }
        bool SendTowards(Vector3 vector3);
        EntityZombie? GetEntity();
        public void Destroy();
        PointF? GetPosition();
        bool IsAlive { get; }
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

        public PointF? GetPosition() => GetEntity()?.GetPosition().ToPoint();

        public bool IsAlive => !GetEntity()?.bDead ?? false;
        
        public void Destroy()
        {
            var entity = GetEntity();
            if (entity == null) return;
            _world.DestroyZombie(this);
        }

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
        
        protected bool Equals(Zombie other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Zombie)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}