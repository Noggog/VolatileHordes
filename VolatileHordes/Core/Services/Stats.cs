using UniLinq;
using VolatileHordes.Players;
using VolatileHordes.Spawning;
using VolatileHordes.Tracking;

namespace VolatileHordes.Core.Services
{
    public class Stats
    {
        private readonly PlayerZoneManager _playerZoneManager;
        private readonly ZombieGroupManager _groupManager;
        private readonly AmbientZombieManager _ambientZombieManager;
        private readonly ZombieCreator _creator;

        public Stats(
            PlayerZoneManager playerZoneManager,
            ZombieGroupManager groupManager,
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
            PlayerZone playerZone;
            if (sender.RemoteClientInfo == null)
            {
                playerZone = _playerZoneManager.Zones.FirstOrDefault();
            }
            else
            {
                playerZone = _playerZoneManager.Zones
                    .FirstOrDefault(x => x.Player.EntityId == sender.RemoteClientInfo.entityId);
            }
            if (playerZone == null || !playerZone.Player.TryGetEntity(out var entity))
            {
                Logger.Warning("No player found to print stats relative to.");
                return;
            }
            _creator.PrintZombieStats();
            var playerPt = entity.position.ToPoint();
            foreach (var group in _groupManager.NormalGroups)
            {
                group.PrintRelativeTo(playerPt);
            }
            Logger.Info("Ambient:");
            _ambientZombieManager.PrintRelativeTo(playerPt);
        }
    }
}