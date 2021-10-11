using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Threading.Tasks;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordePlacer
    {
        private const byte NumPerRow = 5;
        private const double SecondDelay = 2;
        private const float Spacing = 2;
        private readonly TimeManager _time;
        private readonly SpawnRowPerpendicular _spawnRow;

        public WanderingHordePlacer(
            TimeManager time,
            SpawnRowPerpendicular spawnRow)
        {
            _time = time;
            _spawnRow = spawnRow;
        }
        
        public async Task SpawnHorde(PointF pos, PointF target, int size, ZombieGroup group)
        {
            var rows = size / NumPerRow;
            await _time.Interval(TimeSpan.FromSeconds(SecondDelay))
                .Take(rows)
                .Do(_ =>
                {
                    var numToSpawn = checked((byte)Math.Min(size, NumPerRow));
                    size -= numToSpawn;
                    _spawnRow.Spawn(pos, target, numToSpawn, Spacing, group);
                });
        }
    }
}