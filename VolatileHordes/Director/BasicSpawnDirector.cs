using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolatileHordes.Players;
using VolatileHordes.Randomization;
using VolatileHordes.Spawning.WanderingHordes;
using VolatileHordes.Utility;

namespace VolatileHordes.Director
{
    public class BasicSpawnDirector
    {
        private readonly TimeManager timeManager;
        private readonly WanderingHordeDirector wanderingHordeDirector;
        private readonly RandomSource randomSource;
        private readonly FidgetForwardDirector fidgetForwardDirector;
        private readonly PlayerZoneManager playerZoneManager;
        private readonly GameStageCalculator gameStageCalculator;

        public BasicSpawnDirector(
            TimeManager timeManager,
            RandomSource randomSource,
            WanderingHordeDirector wanderingHordeDirector,
            FidgetForwardDirector fidgetForwardDirector,
            PlayerZoneManager playerZoneManager,
            GameStageCalculator gameStageCalculator
        )
        {
            this.timeManager = timeManager;
            this.randomSource = randomSource;
            this.wanderingHordeDirector = wanderingHordeDirector;
            this.fidgetForwardDirector = fidgetForwardDirector;
            this.playerZoneManager = playerZoneManager;
            this.gameStageCalculator = gameStageCalculator;
        }

        public void start()
        {
            Logger.Temp("BasicSpawnDirector.start`Open");
            timeManager.IntervalWithVariance(
                new TimeRange(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2)),
                onNewInterval: timeSpan => Logger.Temp("Will emit in {0}", timeSpan)
            )
                .Subscribe(async x =>
                {
                    var spawnCount =
                        (int)
                        (6
                        + gameStageCalculator.GetGamestage(playerZoneManager.Zones.First().Group).Log("gameStage")
                        * 0.2);

                    Logger.Temp("Spawning. spawnCount:{0}", spawnCount);
                    var randomNumber = randomSource.Get(2);

                    switch (randomNumber)
                    {
                        case 0:
                            await wanderingHordeDirector.Spawn(spawnCount);
                            break;
                        case 1:
                            await fidgetForwardDirector.Spawn(spawnCount);
                            break;
                        default:
                            throw new Exception($"Unhandled case:{randomNumber}");
                    }
                });
        }
    }
}
