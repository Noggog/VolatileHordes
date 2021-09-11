using VolatileHordes.Randomization;
using VolatileHordes.Zones;

namespace VolatileHordes.Spawning
{
    public class SingleTracker
    {
        public static readonly SingleTracker Instance = new();

        public void SpawnSingle()
        {
            var playerZone = PlayerZoneManager.Instance.GetRandom(RandomSource.Instance);
            if (playerZone == null)
            { 
                Logger.Info("No player to spawn next to");
                return;
            }
            ZombieCreator.Instance.CreateZombie(playerZone, playerZone.Center);
        }
    }
}