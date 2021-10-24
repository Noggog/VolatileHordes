using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace VolatileHordes.Players
{
    public class PlayerCountProvider
    {
        private BehaviorSubject<int> _playerCount = new(0);
        public IObservable<int> playerCount;
        public PlayerCountProvider(
            PlayerGameEventsWrapper playerGameEventsWrapper
        )
        {
            playerCount = _playerCount;
            Observable.Merge(
                playerGameEventsWrapper.PlayerAdded.Select(x => new ValueTuple<bool, int>(true, x)),
                playerGameEventsWrapper.PlayerRemoved.Select(x => new ValueTuple<bool, int>(false, x))
            )
            .Subscribe(x =>
            {
                if (x.Item1)
                    _playerCount.OnNext(_playerCount.Value + 1);
                else
                    _playerCount.OnNext(_playerCount.Value - 1);
            });
        }
    }
}
