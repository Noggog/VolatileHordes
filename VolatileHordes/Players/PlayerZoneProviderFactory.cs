using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Players
{
    public class PlayerZoneProviderFactory
    {
        private TimeManager timeManager;
        public PlayerZoneProviderFactory(TimeManager timeManager)
        {
            this.timeManager = timeManager;
        }

        public PlayerZoneProvider create(IPlayer player)
        {
            return new PlayerZoneProvider(timeManager, player);
        }
    }
}
