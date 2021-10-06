using System;
using System.Drawing;
using UnityEngine;
using VolatileHordes.Director;
using VolatileHordes.Zones;

namespace VolatileHordes.Spawning
{
    public class SpawnTarget
    {
        public Vector3 SpawnPoint { get; }
        public PointF TriggerOrigin { get; }
        public PlayerZone Player { get; }

        public SpawnTarget(Vector3 spawnPoint, PlayerZone triggerOrigin)
        {
            SpawnPoint = spawnPoint;
            Player = triggerOrigin;
            TriggerOrigin = triggerOrigin.PlayerLocation;
        }

        public override string ToString()
        {
            return $"{SpawnPoint.ToString()} => {TriggerOrigin}";
        }
    }
}