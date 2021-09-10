using System;
using System.Drawing;
using UnityEngine;

namespace VolatileHordes.Spawning
{
    public struct SpawnTarget : IEquatable<SpawnTarget>
    {
        public readonly Vector3 SpawnPoint;
        public readonly RectangleF SpawnArea;

        public SpawnTarget(Vector3 spawnPoint, RectangleF spawnArea)
        {
            SpawnPoint = spawnPoint;
            SpawnArea = spawnArea;
        }

        public bool Equals(SpawnTarget other)
        {
            return SpawnPoint.Equals(other.SpawnPoint) && SpawnArea.Equals(other.SpawnArea);
        }

        public override bool Equals(object? obj)
        {
            return obj is SpawnTarget other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (SpawnPoint.GetHashCode() * 397) ^ SpawnArea.GetHashCode();
            }
        }
    }
}