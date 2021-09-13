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
        private readonly SpawnRowPerpendicular _spawnRow;

        public WanderingHordeSpawner(SpawnRowPerpendicular spawnRow)
        {
            _spawnRow = spawnRow;
        }
        
        public void SpawnHorde(PointF pos, PointF target, int size)
        {
            var rows = size / NumPerRow;
            TimeManager.Instance.Interval(TimeSpan.FromSeconds(SecondDelay))
                .Take(rows)
                .Subscribe(_ =>
                {
                    var numToSpawn = checked((byte)Math.Min(size, NumPerRow));
                    size -= numToSpawn;
                    _spawnRow.Spawn(pos, target, numToSpawn, Spacing);
                });
        }
    }
}