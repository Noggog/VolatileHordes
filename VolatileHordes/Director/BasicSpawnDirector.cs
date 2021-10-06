using System;
using System.Linq;
using VolatileHordes.Players;
using VolatileHordes.Probability;
using VolatileHordes.Spawning.WanderingHordes;
using VolatileHordes.Utility;

namespace VolatileHordes.Director
{
    public class BasicSpawnDirector
    {
        private readonly TimeManager _timeManager;
        private readonly WanderingHordeSpawner _wanderingHordeSpawner;
        private readonly RandomSource _randomSource;
        private readonly FidgetForwardSpawner _fidgetForwardSpawner;
        private readonly PlayerZoneManager _playerZoneManager;
        private readonly GameStageCalculator _gameStageCalculator;

        public BasicSpawnDirector(
            TimeManager timeManager,
            RandomSource randomSource,
            WanderingHordeSpawner wanderingHordeSpawner,
            FidgetForwardSpawner fidgetForwardSpawner,
            PlayerZoneManager playerZoneManager,
            GameStageCalculator gameStageCalculator
        )
        {
            _timeManager = timeManager;
            _randomSource = randomSource;
            _wanderingHordeSpawner = wanderingHordeSpawner;
            _fidgetForwardSpawner = fidgetForwardSpawner;
            _playerZoneManager = playerZoneManager;
            _gameStageCalculator = gameStageCalculator;
        }

        public void Start()
        {
            Logger.Temp("BasicSpawnDirector.start`Open");
            _timeManager.IntervalWithVariance(
                new TimeRange(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2)),
                onNewInterval: timeSpan => Logger.Temp("Will emit in {0}", timeSpan)
            )
                .Subscribe(async x =>
                {
                    var spawnCount =
                        (int)
                        (6
                        + _gameStageCalculator.GetGamestage(_playerZoneManager.Zones.First().Group).Log("gameStage")
                        * 0.2);

                    Logger.Temp("Spawning. spawnCount:{0}", spawnCount);
                    var randomNumber = _randomSource.Get(2);

                    switch (randomNumber)
                    {
                        case 0:
                            await _wanderingHordeSpawner.Spawn(spawnCount);
                            break;
                        case 1:
                            await _fidgetForwardSpawner.Spawn(spawnCount);
                            break;
                        default:
                            throw new Exception($"Unhandled case:{randomNumber}");
                    }
                });
        }
    }
}
