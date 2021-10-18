using System;
using System.Linq;
using VolatileHordes.Players;
using VolatileHordes.Probability;
using VolatileHordes.Spawning;
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

        public BasicSpawnDirector(
            DirectorSwitch directorSwitch,
            TimeManager timeManager,
            RandomSource randomSource,
            WanderingHordeSpawner wanderingHordeSpawner,
            FidgetForwardSpawner fidgetForwardSpawner,
            PlayerPartiesProvider playerPartiesProvider)
        {
            _timeManager = timeManager;
            _randomSource = randomSource;
            _wanderingHordeSpawner = wanderingHordeSpawner;
            _fidgetForwardSpawner = fidgetForwardSpawner;


            _timeManager.IntervalWithVariance(new TimeRange(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(20)))
                .FlowSwitch(directorSwitch.Enabled)
                .SubscribeAsync(async _ =>
                    {
                        var spawnCount =
                            playerPartiesProvider.playerParties
                                .First()
                                .GameStage
                                .Let(x => 6 + 0.1 * x)
                                .Let(x => (ushort)x);

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
                    },
                    e => Logger.Error("{0} had update error {1}", nameof(BasicSpawnDirector), e));
        }
    }
}
