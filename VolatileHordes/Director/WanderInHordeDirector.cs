using System;
using System.Drawing;
using VolatileHordes.Core.Models;
using VolatileHordes.Probability;
using VolatileHordes.Settings.User.Director;
using VolatileHordes.Spawning;
using VolatileHordes.Spawning.WanderingHordes;

namespace VolatileHordes.Director
{
    public class WanderInHordeDirectorFactory
    {
        private readonly DirectorSwitch _directorSwitch;
        private readonly TimeManager _timeManager;
        private readonly RandomSource _randomSource;
        private readonly ProbabilityList<IHordeSpawner> _spawners;

        public WanderInHordeDirectorFactory(
            DirectorSwitch directorSwitch,
            TimeManager timeManager,
            RandomSource randomSource,
            WanderingHordeSpawner wanderingHordeSpawner,
            FidgetForwardSpawner fidgetForwardSpawner,
            WanderInHordeDirectorSettings settings)
        {
            _directorSwitch = directorSwitch;
            _timeManager = timeManager;
            _randomSource = randomSource;
            _spawners = new();
            _spawners.Add(wanderingHordeSpawner, new UDouble(settings.WanderProbabilityWeight));
            _spawners.Add(fidgetForwardSpawner, new UDouble(settings.FidgetForwardProbabilityWeight));
        }
        
        public WanderInHordeDirector Get(Point allocPoint)
        {
            return new WanderInHordeDirector(
                _directorSwitch,
                _timeManager,
                _randomSource,
                _spawners,
                allocPoint);
        }
    }

    public class WanderInHordeDirector
    {
        public WanderInHordeDirector(
            DirectorSwitch directorSwitch,
            TimeManager timeManager,
            RandomSource randomSource,
            IProbabilityListGetter<IHordeSpawner> hordes,
            Point allocPt)
        {
            timeManager.IntervalWithVariance(new TimeRange(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(20)))
                .FlowSwitch(directorSwitch.Enabled)
                .SubscribeAsync(async _ =>
                    {
                        var spawnCount = (ushort)6; // TODO: Calculate based on GameStage

                        var spawner = hordes.Get(randomSource);
                        await spawner.Spawn(spawnCount, allocPt);
                    },
                    e => Logger.Error("{0} had update error {1}", nameof(WanderInHordeDirector), e));
        }
    }
}
