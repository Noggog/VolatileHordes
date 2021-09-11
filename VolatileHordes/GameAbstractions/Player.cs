using System.Collections.Generic;
using UnityEngine;

namespace VolatileHordes.GameAbstractions
{
    public interface IPlayer
    {
        IEnumerable<Vector3> Bedrolls { get; }
    }

    public class Player : IPlayer
    {
        private readonly EntityPlayer _player;

        public IEnumerable<Vector3> Bedrolls => GetBedrolls();

        public Player(EntityPlayer player)
        {
            _player = player;
        }

        private IEnumerable<Vector3> GetBedrolls()
        {
            for (int i = 0; i < _player.SpawnPoints.Count; ++i)
            {
                yield return _player.SpawnPoints[i].ToVector3();
            }
        }
    }
}