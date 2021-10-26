using System.Linq;
using VolatileHordes.Players;
using VolatileHordes.Settings.User.Allocation;
using VolatileHordes.Settings.User.Director;

namespace VolatileHordes.Allocation
{
    public class GameStageCalculator
    {
        private readonly AllocationSettings _settings;

        public GameStageCalculator(AllocationSettings settings)
        {
            _settings = settings;
        }
        
        public float GetGamestage(PlayerParty playerParty)
        {
            var gameStages = playerParty.players.Values
                .Select(x => x.TryGameStage() ?? default(int?))
                .NotNull()
                .ToArray();
            if (gameStages.Length == 0) return 0;
            if (gameStages.Length == 1) return gameStages[0];
            
            var max = gameStages.Max(x => x);
            float accum = max;
            bool skippedTrigger = false;
            foreach (var gameStage in gameStages)
            {
                if (gameStage == max
                    && !skippedTrigger)
                {
                    skippedTrigger = true;
                    continue;
                }

                accum += gameStage * _settings.AdditionalPlayerGamestagePercentageUsage;
            }

            return accum;
        }
    }
}