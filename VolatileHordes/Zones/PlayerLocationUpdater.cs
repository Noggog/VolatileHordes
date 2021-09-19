using System.Reactive.Linq;

namespace VolatileHordes.Zones
{
    public class PlayerLocationUpdater
    {
        private readonly PlayerZoneManager _zoneManager;

        public PlayerLocationUpdater(
            PlayerZoneManager zoneManager,
            TimeManager time)
        {
            _zoneManager = zoneManager;
            Bootstrapper.GameStarted
                .Select(_ => time.UpdateTicks())
                .Switch()
                .Subscribe(Update,
                    onError: (e) => Logger.Error("{0} update failed: {1}", nameof(PlayerZoneManager), e));
        }

        public void Update()
        {
            var world = GameManager.Instance.World;
            var players = world.Players.dict;

            for (int i = _zoneManager.Zones.Count - 1; i >= 0; i--)
            {
                var ply = _zoneManager.Zones[i];

                if (players.TryGetValue(ply.EntityId, out var ent))
                {
                    _zoneManager.Zones[i] = _zoneManager.UpdatePlayer(ply, ent);
                }
                else
                {
                    // Remove player.
                    ply.Valid = false;
                    _zoneManager.Zones.RemoveAt(i);
                    i--;

                    Logger.Error("Player not in player list: {0}", ply.EntityId);
                }
            }
        }
    }
}