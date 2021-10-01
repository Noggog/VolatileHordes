﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Subjects;
using UniLinq;
using VolatileHordes.AiPackages;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Utility;

namespace VolatileHordes.Tracking
{
    public interface IZombieGroup : IEnumerable<IZombie>
    {
        int Id { get; }
        DateTime SpawnTime { get; }
        int Count { get; }
        PointF? Target { get; set; }
        IAiPackage? AiPackage { get; }
        void Add(IEnumerable<IZombie> zombies);
        void Add(IZombie zombie);
        void AddForDisposal(IDisposable disposable);
        bool Remove(IZombie zombie);
        void Dispose();
        int NumAlive();
        string? ToString();
        bool ContainsZombie(IZombie zombie);
        void Destroy();
        PointF? GetGeneralLocation();
        IObservable<PointF?> FollowTarget();
        void PrintRelativeTo(PointF pt);
    }

    public class ZombieGroup : IDisposable, IDisposableBucket, IZombieGroup
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

            // Average all point locations for center of mass
            PointF? ret = null;
            foreach (var zomb in _zombies.Values)
            {
                if (!zomb.IsAlive) continue;
                var pos = zomb.GetPosition();
                if (pos == null) continue;
                if (ret == null)
                {
                    ret = pos;
                }
                else
                {
                    ret = pos.Value.Average(ret.Value);
                }
            }

            return ret;
        }

        public IObservable<PointF?> FollowTarget() => _target;

        public void PrintRelativeTo(PointF pt)
        {
            Logger.Info("{0}", this);
            var generalLoc = GetGeneralLocation();
            Logger.Info("General Location {0}, {1} away", generalLoc, generalLoc?.AbsDistance(pt));
            foreach (var zombie in _zombies.Values
                .OrderBy(x => x.Id))
            {
                if (!zombie.IsAlive) continue;
                zombie.PrintRelativeTo(pt);
            }
        }
    }
}