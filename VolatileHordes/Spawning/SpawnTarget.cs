using System.Drawing;
using UnityEngine;
using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Spawning
{
    public class SpawnTarget
    {
        public Vector3 SpawnPoint { get; }
        public PointF TriggerOrigin { get; }
        public Player Player { get; }

        public SpawnTarget(Vector3 spawnPoint, Player triggerOrigin)
        {
            SpawnPoint = spawnPoint;
            Player = triggerOrigin;
            TriggerOrigin = triggerOrigin.location;
        }

        public override string ToString()
        {
            return $"{SpawnPoint} => {TriggerOrigin}";
        }
    }
}