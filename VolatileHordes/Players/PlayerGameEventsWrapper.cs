using System;
using System.Reactive.Subjects;

namespace VolatileHordes.Players
{
    public class PlayerGameEventsWrapper
    {
        public PlayerGameEventsWrapper()
        {

        }

        // # Input
        public void PlayerSpawnedInWorld(ClientInfo? _cInfo, RespawnType _respawnReason, Vector3i _pos)
        {
            Logger.Debug("PlayerSpawnedInWorld \"{0}\", \"{1}\", \"{2}\"", _cInfo?.ToString() ?? "null", _respawnReason, _pos);
            switch (_respawnReason)
            {
                case RespawnType.NewGame:
                // case RespawnType.LoadedGame:
                case RespawnType.EnterMultiplayer:
                case RespawnType.JoinMultiplayer:
                    _PlayerAdded.OnNext(
                        GetPlayerEntityId(_cInfo)
                    );
                    break;
            }
        }

        public void PlayerDisconnected(ClientInfo? _cInfo, bool _bShutdown)
        {
            Logger.Debug("PlayerDisconnected \"{0}\", \"{1}\"", _cInfo?.ToString() ?? "null", _bShutdown);
            _PlayerRemoved.OnNext(
                GetPlayerEntityId(_cInfo)
            );
        }

        // # Internal
        static int GetPlayerEntityId(ClientInfo? _cInfo)
        {
            if (_cInfo != null)
                return _cInfo.entityId;

            // On a local host this is set to null, grab id from player list.
            var player = GameManager.Instance.World.Players.list[0];

            return player.entityId;
        }

        // # Output
        private Subject<int> _PlayerAdded { get; } = new();
        public IObservable<int> PlayerAdded { get { return _PlayerAdded; } }
        private Subject<int> _PlayerRemoved { get; } = new();
        public IObservable<int> PlayerRemoved { get { return _PlayerRemoved; } }
    }
}
