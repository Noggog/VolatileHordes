using System.Collections.Generic;
using UniLinq;
using UnityEngine;
using VolatileHordes.Players;

namespace VolatileHordes.GameAbstractions
{
    public interface IPlayer
    {
        int EntityId { get; }
        public PlayerZone PlayerZone { get; }
        EntityPlayer? TryGetEntity();
        bool TryGetEntity(out EntityPlayer player);
        IEnumerable<Vector3> Bedrolls { get; }
        int? TryGameStage();
    }

    public class Player : IPlayer
    {
        private readonly IWorld _world;
        public int EntityId { get; }
        public PlayerZone PlayerZone { get; }

        public IEnumerable<Vector3> Bedrolls => GetBedrolls();

        public Player(
            IWorld world,
            int id)
        {
            _world = world;
            EntityId = id;

            this.PlayerZone = new PlayerZone(this);
        }

        private IEnumerable<Vector3> GetBedrolls()
        {
            var entity = TryGetEntity();
            if (entity == null) yield break;
            for (int i = 0; i < entity.SpawnPoints.Count; ++i)
            {
                yield return entity.SpawnPoints[i].ToVector3();
            }
        }

        public EntityPlayer? TryGetEntity()
        {
            var entity = _world.GetEntity(EntityId);
            return entity as EntityPlayer;
        }

        public bool TryGetEntity(out EntityPlayer player)
        {
            player = TryGetEntity()!;
            return player != null;
        }
        
        public int? TryGameStage()
        {
            return TryGetEntity()?.gameStage;
        }
    }
}