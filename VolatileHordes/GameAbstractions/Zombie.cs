using UnityEngine;

namespace VolatileHordes.GameAbstractions
{
    public interface IZombie
    {
        void SendTowards(Vector3 vector3);
    }

    public class Zombie : IZombie
    {
        private readonly EntityZombie _zombie;

        public Zombie(EntityZombie zombie)
        {
            _zombie = zombie;
        }

        public void SendTowards(Vector3 vector3)
        {
            _zombie.SetInvestigatePosition(vector3, 6000, false);
        }

        public override string ToString()
        {
            return _zombie.ToString();
        }
    }
}