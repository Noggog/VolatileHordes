using UniLinq;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Players;
using VolatileHordes.Tracking;

namespace VolatileHordes.Core.Services
{
    public class Stats
    {
        private readonly PlayerPartiesProvider playerPartiesProvider;
        private readonly ZombieGroupManager _groupManager;
        private readonly AmbientZombieManager _ambientZombieManager;
        private readonly LimitManager _limits;

        public Stats(
            PlayerPartiesProvider playerPartiesProvider,
            ZombieGroupManager groupManager,
            AmbientZombieManager ambientZombieManager,
            LimitManager limits)
        {
            this.playerPartiesProvider = playerPartiesProvider;
            _groupManager = groupManager;
            _ambientZombieManager = ambientZombieManager;
            _limits = limits;
        }

        public void Print(CommandSenderInfo sender)
        {
            Player player;
            if (sender.RemoteClientInfo == null)
            {
                player = playerPartiesProvider.players.FirstOrDefault();
            }
            else
            {
                player = playerPartiesProvider.players
                    .FirstOrDefault(x => x.EntityId == sender.RemoteClientInfo.entityId);
            }
            if (player == null || !player.TryGetEntity(out var entity))
            {
                Logger.Warning("No player found to print stats relative to.");
                return;
            }
            _limits.PrintZombieStats();
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