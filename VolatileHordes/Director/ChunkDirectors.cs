using System;
using System.Drawing;

namespace VolatileHordes.Director
{
    public class ChunkDirectorFactory
    {
        private readonly WanderInHordeDirectorFactory _wanderInHordeDirectorFactory;

        public ChunkDirectorFactory(WanderInHordeDirectorFactory wanderInHordeDirectorFactory)
        {
            _wanderInHordeDirectorFactory = wanderInHordeDirectorFactory;
        }
        
        public ChunkDirectors Create(Point allocPoint)
        {
            return new ChunkDirectors(
                _wanderInHordeDirectorFactory,
                allocPoint);
        }
    }
    
    public class ChunkDirectors : IDisposable
    {
        public Point AllocPoint { get; }

        public DateTime PlayerLastSeen { get; set; }
        
        public ChunkDirectors(
            WanderInHordeDirectorFactory wanderInHordeDirectorFactory, 
            Point allocPoint)
        {
            AllocPoint = allocPoint;
        }

        public void Dispose()
        {
        }
    }
}