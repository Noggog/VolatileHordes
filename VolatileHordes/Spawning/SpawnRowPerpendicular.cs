using System;
using System.Drawing;
using UnityEngine;

namespace VolatileHordes.Spawning
{
    public class SpawnRowPerpendicular
    {
        private readonly SingleTrackerSpawner _singleTrackerSpawner;

        public SpawnRowPerpendicular(SingleTrackerSpawner singleTrackerSpawner)
        {
            _singleTrackerSpawner = singleTrackerSpawner;
        }

        public Vector3 Perpendicular(PointF pt1, PointF pt2)
        {
            var lineToTarget = pt1.WithHeight(0) - pt2.WithHeight(0);
            return Vector3.Cross(lineToTarget, Vector3.up).normalized;
        }
        
        public void Spawn(PointF spawnLocation, PointF target, byte number, float spacing)
        {
            if (number == 0) return;
            _singleTrackerSpawner.Spawn(spawnLocation, target);

            if (number == 1) return;

            var numPerSide = number / 2.0f;

            var perpendicular = Perpendicular(spawnLocation, target);

            var numToSpawn = Mathf.Ceil(numPerSide);
            for (int i = 1; i <= numToSpawn; i++)
            {
                _singleTrackerSpawner.Spawn((spawnLocation.WithHeight(0) + perpendicular * spacing * i).ToPoint(), target);
            }
            
            numToSpawn = Mathf.Floor(numPerSide);
            for (int i = 1; i <= numToSpawn; i++)
            {
                _singleTrackerSpawner.Spawn((spawnLocation.WithHeight(0) + perpendicular * spacing * i * -1).ToPoint(), target);
            }
        }
    }
}