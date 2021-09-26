using System.Collections.Generic;
using System.Threading.Tasks;

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
                case "wander":
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
                case "redirect":
                {
                    Logger.Info("Artificially redirecting");
                    Container.RoamControl.Redirect.Fire();
                    break;
                }
                case "seeker":
                {
                    Logger.Info("Artificially spawning a seeker squad");
                    Container.SeekerGroupDirector.Spawn();
                    break;
                }
                case "runner":
                {
                    Logger.Info("Artificially spawning a runner");
                    Container.SingleRunnerDirector.Spawn(nearPlayer: true);
                    break;
                }
                case "wipe":
                {
                    Logger.Info("Wiping all tracked zombies");
                    Container.GroupManager.DestroyAll();
                    break;
                }
                case "players":
                {
                    Container.PlayerZoneManager.Print();
                    break;
                }
                default:
                    break;
            }
        }
    }
}