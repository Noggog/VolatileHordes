using System.Collections.Generic;
using System.Threading.Tasks;
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
            ExecuteTask(paramList)
                .FireAndForget(x => Log.Error("Error running command {0}", x));
        }

        private async Task ExecuteTask(List<string> paramList)
        {
            if (paramList.Count < 1)
                return;

            switch (paramList[0].ToLower())
            {
                case "stats":
                {
                    Container.ZombieCreator.PrintZombieStats();
                    break;
                }
                case "wandering":
                {
                    Logger.Info("Artificially spawning a wandering horde");
                    if (paramList.Count > 1 && int.TryParse(paramList[1], out var size))
                    {
                        await Container.WanderingHordeDirector.Spawn(size);
                    }
                    else
                    {
                        await Container.WanderingHordeDirector.Spawn();
                    }
                    
                    break;
                }
                case "single-tracker":
                {
                    Logger.Info("Artificially spawning a single tracking zombie");
                    Container.SingleTrackerDirector.SpawnSingle();
                    break;
                }
                default:
                    break;
            }
        }
    }
}