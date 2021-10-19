using VolatileHordes.GameAbstractions;

namespace VolatileHordes.Players
{
    public class PlayerFactory
    {
        private IWorld world;
        private PlayerZoneProviderFactory playerZoneProviderFactory;
        public PlayerFactory(
            IWorld world,
            PlayerZoneProviderFactory playerZoneProviderFactory)
        {
            this.world = world;
            this.playerZoneProviderFactory = playerZoneProviderFactory;
        }

        public Player create(int id)
        {
            return new Player(world, playerZoneProviderFactory, id);
        }
    }
}
