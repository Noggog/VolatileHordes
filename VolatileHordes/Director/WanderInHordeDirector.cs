using System;
using System.Drawing;
using System.Linq;
using VolatileHordes.Allocation;
using VolatileHordes.Players;
using VolatileHordes.Probability;
using VolatileHordes.Spawning;
using VolatileHordes.Spawning.WanderingHordes;

namespace VolatileHordes.Director
{
    public class WanderInHordeDirectorFactory
    {
        private readonly DirectorSwitch _directorSwitch;
        private readonly TimeManager _timeManager;
        private readonly RandomSource _randomSource;
        private readonly WanderingHordeSpawner _wanderingHordeSpawner;
        private readonly FidgetForwardSpawner _fidgetForwardSpawner;

        public WanderInHordeDirectorFactory(
            DirectorSwitch directorSwitch,
            TimeManager timeManager,
            RandomSource randomSource,
            WanderingHordeSpawner wanderingHordeSpawner,
            FidgetForwardSpawner fidgetForwardSpawner)
        {
            _directorSwitch = directorSwitch;
            _timeManager = timeManager;
            _randomSource = randomSource;
            _wanderingHordeSpawner = wanderingHordeSpawner;
            _fidgetForwardSpawner = fidgetForwardSpawner;
        }
        
        public WanderInHordeDirector Get(Point allocPoint)
        {
            return new WanderInHordeDirector(
                _directorSwitch,
                _timeManager,
                _randomSource,
                _wanderingHordeSpawner,
                _fidgetForwardSpawner,
                allocPoint);
        }
    }

    public class WanderInHordeDirector
    {
        public WanderInHordeDirector(
            DirectorSwitch directorSwitch,
            TimeManager timeManager,
            RandomSource randomSource,
            WanderingHordeSpawner wanderingHordeSpawner,
            FidgetForwardSpawner fidgetForwardSpawner,
            Point allocPt)
        {
            timeManager.IntervalWithVariance(new TimeRange(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(20)))
                .FlowSwitch(directorSwitch.Enabled)
                .SubscribeAsync(async _ =>
                    {
                        var spawnCount = (ushort)6; // TODO: Calculate based on GameStage

                        var randomNumber = randomSource.Get(2);

                        switch (randomNumber)
                        {
                            case 0:
                                await wanderingHordeSpawner.Spawn(spawnCount, allocPt);
                                break;
                            case 1:
                                await fidgetForwardSpawner.Spawn(spawnCount, allocPt);
                                break;
                            default:
                                throw new Exception($"Unhandled case:{randomNumber}");
                        }
                    },
                    e => Logger.Error("{0} had update error {1}", nameof(WanderInHordeDirector), e));
        }
    }
}
