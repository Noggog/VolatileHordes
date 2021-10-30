using System;
using System.Drawing;
using UnityEngine;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Players;

namespace VolatileHordes.Spawning
{
    public record SpawnTarget(Vector3 SpawnPoint, PointF Target, PlayerZone Player);
    public record PlayerSpawn(Vector3 SpawnPoint, PlayerZone Player);
}