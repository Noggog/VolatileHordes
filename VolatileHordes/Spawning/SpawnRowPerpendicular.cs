using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using VolatileHordes.Control;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Tracking;

namespace VolatileHordes.Spawning
{
    public class SpawnRowPerpendicular
    {
        private readonly ZombieCreator _zombieCreator;
        private readonly ZombieControl _control;

        public SpawnRowPerpendicular(
            ZombieCreator zombieCreator,
            ZombieControl control)
        {
            _zombieCreator = zombieCreator;
            _control = control;
        }

        public Vector3 Perpendicular(PointF pt1, PointF pt2)
        {
            var lineToTarget = pt1.WithHeight(0) - pt2.WithHeight(0);
            return Vector3.Cross(lineToTarget, Vector3.up).normalized;
        }
        
        public void Spawn(PointF spawnLocation, PointF target, byte number, float spacing, ZombieGroup group)
        {
            if (number == 0) return;

            _zombieCreator.CreateZombie(spawnLocation, group);

            if (number == 1) return;

            var numPerSide = Mathf.Floor(number / 2.0f);

            var perpendicular = Perpendicular(spawnLocation, target);

            for (int i = 1; i <= numPerSide; i++)
            {
                _zombieCreator.CreateZombie((spawnLocation.WithHeight(0) + perpendicular * spacing * i).ToPoint(), group);
            }
            
            for (int i = 1; i <= numPerSide; i++)
            {
                _zombieCreator.CreateZombie((spawnLocation.WithHeight(0) + perpendicular * spacing * i * -1).ToPoint(), group);
            }
            
            _control.SendGroupTowards(group, target);
        }
    }
}