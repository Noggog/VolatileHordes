using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using VolatileHordes.AiPackages;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Tracking;

namespace VolatileHordes.NoiseViewer.Simulations
{
    public class SimulationZombieGroup : IZombieGroup
    {
        public int Id { get; }
        public DateTime SpawnTime { get; }
        public int Count { get; }
        public PointF? Target { get; set; }
        public IAiPackage? AiPackage { get; }
        public void Add(IEnumerable<IZombie> zombies)
        {
            throw new NotImplementedException();
        }

        public void Add(IZombie zombie)
        {
            throw new NotImplementedException();
        }

        public void AddForDisposal(IDisposable disposable)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IZombie zombie)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int NumAlive()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<IZombie> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool ContainsZombie(IZombie zombie)
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public PointF? GetGeneralLocation()
        {
            return Target;
        }

        public IObservable<PointF?> FollowTarget()
        {
            throw new NotImplementedException();
        }

        public void PrintRelativeTo(PointF pt)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}