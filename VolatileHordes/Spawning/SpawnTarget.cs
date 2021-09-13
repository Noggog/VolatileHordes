using System;
using System.Drawing;
using UnityEngine;

namespace VolatileHordes.Spawning
{
    public class SpawnTarget
    {
        public Vector3 SpawnPoint { get; }
        public PointF TriggerOrigin { get; }

        public SpawnTarget(Vector3 spawnPoint, PointF triggerOrigin)
        {
            SpawnPoint = spawnPoint;
            TriggerOrigin = triggerOrigin;
        }
    }
}