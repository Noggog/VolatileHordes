using System;
using System.Collections.Generic;
using System.Drawing;
using VolatileHordes.Allocation;
using VolatileHordes.Core.Extensions;
using VolatileHordes.Players;

namespace VolatileHordes.Director
{
    public class DirectorManager
    {
        private readonly Dictionary<Point, ChunkDirectors> _chunkDirectors = new();
        private readonly List<Point> _chunksToDelete = new();

        public DirectorManager(
            TimeManager time,
            AllocationManager allocationManager,
            ChunkDirectorFactory chunkDirectorFactory,
            PlayerZoneManager playerZoneManager)
        {
            time.Interval(TimeSpan.FromSeconds(5))
                .Subscribe(_ =>
                {
                    var now = DateTime.Now;
                    
                    foreach (var player in playerZoneManager.Zones)
                    {
                        var allocPoint = allocationManager.GetAllocationBucket(player.Center);
                        var chunkDirectors = _chunkDirectors.TryCreate(allocPoint, chunkDirectorFactory.Create);
                        chunkDirectors.PlayerLastSeen = now;
                    }

                    _chunksToDelete.Clear();
                    foreach (var chunkDir in _chunkDirectors.Values)
                    {
                        if (now - chunkDir.PlayerLastSeen > TimeSpan.FromSeconds(60))
                        {
                            _chunksToDelete.Add(chunkDir.AllocPoint);
                        }
                    }

                    foreach (var toDelete in _chunksToDelete)
                    {
                        _chunkDirectors.GetAndRemove(toDelete).Dispose();
                    }
                });
        }
    }
}