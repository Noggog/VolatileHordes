using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes
{
    public class ZombieGroup : IDisposable
    {
        private static int _nextId;
        public int Id { get; }

        private readonly List<IDisposable> _behaviors = new();
        
        public DateTime SpawnTime { get; } = DateTime.Now;
        public List<IZombie> Zombies { get; } = new();
        
        public PointF? Target { get; set; }
        
        public ZombieGroup()
        {
            Id = _nextId++;
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
    }
}