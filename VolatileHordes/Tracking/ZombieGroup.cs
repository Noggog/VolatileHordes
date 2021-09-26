using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using VolatileHordes.AiPackages;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Utility;

namespace VolatileHordes.Tracking
{
    public class ZombieGroup : IDisposable, IDisposableBucket
    {
        private static int _nextId;
        public int Id { get; }

        private readonly List<IDisposable> _behaviors = new();
        
        public DateTime SpawnTime { get; } = DateTime.Now;
        public List<IZombie> Zombies { get; } = new();

        private readonly BehaviorSubject<PointF?> _target = new(null);

        public PointF? Target
        {
            get => _target.Value;
            set => _target.OnNext(value);
        }
        
        public IAiPackage? AiPackage { get; }
        
        public ZombieGroup(IAiPackage? package)
        {
            Id = _nextId++;
            AiPackage = package;
        }

        public void AddForDisposal(IDisposable disposable)
        {
            _behaviors.Add(disposable);
        }

        public void Dispose()
        {
            foreach (var behavior in _behaviors)
            {
                behavior.Dispose();
            }
        }

        public int NumAlive() => Zombies
            .Select(z => z.GetEntity())
            .NotNull()
            .Count(e => !e.IsDead());

        public override string ToString()
        {
            return $"{nameof(ZombieGroup)}-{Id} ({NumAlive()}/{Zombies.Count})";
        }

        public void Destroy()
        {
            foreach (var zombie in Zombies)
            {
                zombie.Destroy();
            }
        }

        public PointF? GetGeneralLocation()
        {
            if (Zombies.Count == 0) return null;
            if (Zombies.Count == 1) return Zombies[0].GetPosition();

            RectangleF? rect = null;
            foreach (var zomb in Zombies)
            {
                var pos = zomb.GetPosition();
                if (pos == null) continue;
                if (rect == null)
                {
                    rect = new RectangleF(pos.Value, new SizeF(1, 1));
                }
                else
                {
                    rect = rect.Value.Absorb(pos.Value);
                }
            }

            return rect?.GetCenter();
        }

        public IObservable<PointF?> FollowTarget() => _target;
    }
}