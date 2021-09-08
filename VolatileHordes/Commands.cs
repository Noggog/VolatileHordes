using System.Collections.Generic;
using VolatileHordes.Spawning;
using VolatileHordes.Spawning.WanderingHordes;

namespace VolatileHordes
{
    public class Commands : ConsoleCmdAbstract
    {
        public override string[] GetCommands()
        {
            return new []{"vhordes"};
        }

        public override string GetDescription()
        {
            return Constants.ModName;
        }

        public override void Execute(List<string> paramList, CommandSenderInfo _)
        {
            if (paramList.Count < 1)
                return;

            switch (paramList[0].ToLower())
            {
                case "wandering":
                {
                    Logger.Info("Artificially spawning a wandering horde");
                    WanderingHordeDirector.Instance.SpawnHorde();
                    break;
                }
                case "single-tracker":
                {
                    Logger.Info("Artificially spawning a single tracking zombie");
                    SingleTracker.Instance.SpawnSingle();
                    break;
                }
                default:
                    break;
            }
        }
    }
}