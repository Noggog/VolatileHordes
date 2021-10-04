using UniLinq;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;
using VolatileHordes.Zones;

namespace VolatileHordes
{
    public class Stats
    {
        private readonly PlayerZoneManager _playerZoneManager;
        private readonly GroupManager _groupManager;
        private readonly AmbientZombieManager _ambientZombieManager;
        private readonly ZombieCreator _creator;

        public Stats(
            PlayerZoneManager playerZoneManager,
            GroupManager groupManager,
            AmbientZombieManager ambientZombieManager,
            ZombieCreator creator)
        {
            _playerZoneManager = playerZoneManager;
            _groupManager = groupManager;
            _ambientZombieManager = ambientZombieManager;
            _creator = creator;
        }

        public void Print(CommandSenderInfo sender)
        {
            var playerZone = _playerZoneManager.Zones
                .FirstOrDefault(x => x.Player.EntityId == sender.RemoteClientInfo.entityId);
            if (playerZone == null || playerZone.Player.TryGetEntity(out var entity))
            {
                Logger.Warning("No player found to print stats relative to.");
                return;
            }
            _creator.PrintZombieStats();
            var playerPt = entity.position.ToPoint();
            foreach (var group in _groupManager.Groups)
            {
                group.PrintRelativeTo(playerPt);
            }
            Logger.Info("Ambient:");
            _ambientZombieManager.PrintRelativeTo(playerPt);
        }
    }
}