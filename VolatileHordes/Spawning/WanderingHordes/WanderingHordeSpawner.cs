using System;
using System.Drawing;
using System.Reactive.Linq;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordeSpawner
    {
        private const byte NumPerRow = 5;
        private const double SecondDelay = 2;
        private const float Spacing = 2;
        private readonly TimeManager _time;
        private readonly SpawnRowPerpendicular _spawnRow;

        public WanderingHordeSpawner(
            TimeManager time,
            SpawnRowPerpendicular spawnRow)
        {
            _time = time;
            _spawnRow = spawnRow;
        }
        
        public void SpawnHorde(PointF pos, PointF target, int size, ZombieGroup? group)
        {
            var rows = size / NumPerRow;
            _time.Interval(TimeSpan.FromSeconds(SecondDelay))
                .Take(rows)
                .Subscribe(_ =>
                {
                    var numToSpawn = checked((byte)Math.Min(size, NumPerRow));
                    size -= numToSpawn;
                    _spawnRow.Spawn(pos, target, numToSpawn, Spacing, group);
                });
        }
    }
}