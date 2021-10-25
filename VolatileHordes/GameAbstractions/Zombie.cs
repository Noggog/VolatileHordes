using System.Drawing;
using UnityEngine;
using VolatileHordes.Utility;

namespace VolatileHordes.GameAbstractions
{
    public interface IZombie
    {
        int Id { get; }
        bool Destroyed { get; }
        bool SendTowards(PointF pt);
        EntityZombie? GetEntity();
        PointF? GetTarget();
        public void Destroy();
        PointF? GetPosition();
        Vector3? GetRotation();
        bool IsAlive { get; }
        bool IsDespawned { get; }
        bool IsSleeper { get; }
        void PrintRelativeTo(PointF pt);
    }

    public class Zombie : IZombie
    {
        private readonly IWorld _world;
        public int Id { get; }
        public bool Destroyed { get; private set; }

        public Zombie(
            IWorld world,
            int entityId)
        {
            _world = world;
            Id = entityId;
        }

        public EntityZombie? GetEntity() => _world.GetEntity(Id) as EntityZombie;
        public PointF? GetTarget() => GetEntity()?.InvestigatePosition.ToPoint();

        public PointF? GetPosition() => GetEntity()?.GetPosition().ToPoint();

        public Vector3? GetRotation() => GetEntity()?.rotation;

        public bool IsAlive => !GetEntity()?.bDead ?? false;
        public bool IsDespawned => GetEntity()?.IsDespawned ?? true;
        public bool IsSleeper => GetEntity()?.IsSleeper ?? false;

        public void Destroy()
        {
            if (Destroyed) return;
            _world.DestroyZombie(this);
            Destroyed = true;
        }

        public bool SendTowards(PointF pt)
        {
            var entity = GetEntity();
            if (entity == null) return false;
            entity.SetInvestigatePosition(_world.GetWorldVector(pt), 6000, false);
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

        public void PrintRelativeTo(PointF pt)
        {
            var loc = GetPosition();
            Logger.Info("{0} at {1}, {2} away", Id, loc, loc?.AbsDistance(pt));
        }
    }
}