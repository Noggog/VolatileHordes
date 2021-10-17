using System;
using System.Reactive.Subjects;

namespace VolatileHordes.Players
{
    public class PlayerGameEventsWrapper
    {
        public static PlayerGameEventsWrapper Create()
        {
            return new PlayerGameEventsWrapper(ModEvents.PlayerSpawnedInWorld, ModEvents.PlayerDisconnected);
        }

        public PlayerGameEventsWrapper(
            ModEvent<ClientInfo, RespawnType, Vector3i> playerSpawnedInWorldEvent,
            ModEvent<ClientInfo, bool> playerDisconnectedEvent
        )
        {
            playerSpawnedInWorldEvent.RegisterHandler((clientInfo, respawnType, vector) =>
            {
                Logger.Debug("PlayerSpawnedInWorld \"{0}\", \"{1}\", \"{2}\"", clientInfo?.ToString() ?? "null", respawnType, vector);
                switch (respawnType)
                {
                    case RespawnType.NewGame:
                    // case RespawnType.LoadedGame:
                    case RespawnType.EnterMultiplayer:
                    case RespawnType.JoinMultiplayer:
                        PlayerAddedSubject.OnNext(
                            GetPlayerEntityId(clientInfo)
                        );
                        break;
                }
            });
            playerDisconnectedEvent.RegisterHandler((clientInfo, isShutdown) =>
            {
                Logger.Debug("PlayerDisconnected \"{0}\", \"{1}\"", clientInfo?.ToString() ?? "null", isShutdown);
                PlayerRemovedSubject.OnNext(
                    GetPlayerEntityId(clientInfo)
                );
            });
        }

        // # Internal
        private static int GetPlayerEntityId(ClientInfo? _cInfo)
        {
            if (_cInfo != null)
                return _cInfo.entityId;

            // On a local host this is set to null, grab id from player list.
            var player = GameManager.Instance.World.Players.list[0];

            return player.entityId;
        }

        // # Output
        private Subject<int> PlayerAddedSubject { get; } = new();
        public IObservable<int> PlayerAdded { get { return PlayerAddedSubject; } }
        private Subject<int> PlayerRemovedSubject { get; } = new();
        public IObservable<int> PlayerRemoved { get { return PlayerRemovedSubject; } }
    }
}
