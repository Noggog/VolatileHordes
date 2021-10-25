using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using VolatileHordes.Director;
using VolatileHordes.GameAbstractions;
using VolatileHordes.Probability;

namespace VolatileHordes.Players
{
    public class PlayersProvider
    {
        /*
         * All players, grouped into parties
         */
        public List<PlayerParty> playerParties = new();
        /*
         * All players
         */
        public IEnumerable<Player> players => playerParties.SelectMany(x => x.players);
        /*
         * The number of players, as an observable
         */
        private BehaviorSubject<int> _playerCount = new(0);
        public IObservable<int> playerCount { get { return _playerCount; } }
        public PlayersProvider(
            PlayerGameEventsWrapper playerGameEventsWrapper,
            IWorld world,
            GameStageCalculator gameStageCalculator,
            RandomSource randomSource
        )
        {
            playerParties.Add(new(randomSource, gameStageCalculator, world));
            Observable.Merge(
                playerGameEventsWrapper.PlayerAdded.Select(x => new ValueTuple<bool, int>(true, x)),
                playerGameEventsWrapper.PlayerRemoved.Select(x => new ValueTuple<bool, int>(false, x))
            )
            .Subscribe(x =>
            {
                if (x.Item1)
                {
                    playerParties[0].playersDictionary.Add(x.Item2, new Player(world, x.Item2));
                    _playerCount.OnNext(playerParties[0].playersDictionary.Count);
                }
                else
                {
                    playerParties[0].playersDictionary.Remove(x.Item2);
                    _playerCount.OnNext(playerParties[0].playersDictionary.Count);
                }
            });
        }

        /*
         * Logs information about all player parties
         */
        public void Log()
        {
            foreach (var player in players)
            {
                string playerPos;
                if (player.TryGetEntity(out var entity))
                {
                    playerPos = entity.GetPosition().ToString();
                }
                else
                {
                    playerPos = "<MISSING>";
                }
                Logger.Info("Player zone center {0}.  Player currently at {1}", player.location, playerPos);
            }
        }
    }
}
