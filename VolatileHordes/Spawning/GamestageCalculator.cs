using VolatileHordes.Zones;

namespace VolatileHordes.Spawning
{
    public class GamestageCalculator
    {
        private readonly PlayerZoneManager _playerZoneManager;

        public GamestageCalculator(PlayerZoneManager playerZoneManager)
        {
            _playerZoneManager = playerZoneManager;
        }

        public int GetEffectiveGamestage()
        {
            return 0;
        }
    }
}