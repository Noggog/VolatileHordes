using VolatileHordes.Zones;

namespace VolatileHordes.Spawning
{
    public class GamestageCalculator
    {
        private readonly PlayerZoneManager _playerZoneManager;
        
        public static readonly GamestageCalculator Instance = new(PlayerZoneManager.Instance);

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