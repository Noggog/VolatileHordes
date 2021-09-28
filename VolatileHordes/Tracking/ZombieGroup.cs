using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Subjects;
using VolatileHordes.AiPackages;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Utility;

namespace VolatileHordes.Tracking
{
    public class ZombieGroup : IDisposable, IDisposableBucket, IEnumerable<IZombie>
    {
        private static int _nextId;
        public int Id { get; }

        private readonly List<IDisposable> _behaviors = new();
        
        public DateTime SpawnTime { get; } = DateTime.Now;

        private readonly Dictionary<int, IZombie> _zombies = new();

        private readonly BehaviorSubject<PointF?> _target = new(null);

        public int Count => _zombies.Count;

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

        public void Add(IEnumerable<IZombie> zombies)
        {
            foreach (var zombie in zombies)
            {
                Add(zombie);
            }
        }

        public void Add(IZombie zombie)
        {
            _zombies[zombie.Id] = zombie;
        }

        public void AddForDisposal(IDisposable disposable)
        {
            _behaviors.Add(disposable);
        }

        public bool Remove(IZombie zombie)
        {
            return _zombies.Remove(zombie.Id);
        }

        public void Dispose()
        {
            foreach (var behavior in _behaviors)
            {
                behavior.Dispose();
            }
        }

        public int NumAlive() => _zombies.Values
            .Select(z => z.GetEntity())
            .NotNull()
            .Count(e => !e.IsDead());

        public override string ToString()
        {
            return $"{nameof(ZombieGroup)}-{Id} ({NumAlive()}/{_zombies.Count})";
        }

        public IEnumerator<IZombie> GetEnumerator() => _zombies.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool ContainsZombie(IZombie zombie) => _zombies.ContainsKey(zombie.Id);

        public void Destroy()
        {
            foreach (var zombie in _zombies.Values)
            {
                zombie.Destroy();
            }
        }

        public PointF? GetGeneralLocation()
        {
            if (_zombies.Values.Count == 0) return null;
            if (_zombies.Values.Count == 1) return _zombies.Values.First().GetPosition();

            RectangleF? rect = null;
            foreach (var zomb in _zombies.Values)
            {
                if (!zomb.IsAlive) continue;
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