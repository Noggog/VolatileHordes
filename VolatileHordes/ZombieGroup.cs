using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using VolatileHordes.AiPackages;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Utility;

namespace VolatileHordes
{
    public class ZombieGroup : IDisposable, IDisposableBucket
    {
        private static int _nextId;
        public int Id { get; }

        private readonly List<IDisposable> _behaviors = new();
        
        public DateTime SpawnTime { get; } = DateTime.Now;
        public List<IZombie> Zombies { get; } = new();
        
        public PointF? Target { get; set; }
        
        public IAiPackage? AiPackage { get;}
        
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
    }
}