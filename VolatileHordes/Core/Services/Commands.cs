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

        public override void Execute(List<string> paramList, CommandSenderInfo sender)
        {
            ExecuteTask(paramList, sender)
                .FireAndForget(x => Log.Error("Error running command {0}", x));
        }

        private async Task ExecuteTask(List<string> paramList, CommandSenderInfo sender)
        {
            if (paramList.Count < 1)
                return;

            switch (paramList[0].ToLower())
            {
                case "stats":
                {
                    Container.Stats.Print(sender);
                    break;
                }
                case "wander":
                {
                    if (paramList.Count > 1 && ushort.TryParse(paramList[1], out var size))
                    {
                        Logger.Info("Artificially spawning a wandering horde of size {0}", size);
                        await Container.WanderingHordeSpawner.Spawn(size);
                    }
                    
                    Logger.Info("Need to supply desired horde size");
                    break;
                }
                case "redirect":
                {
                    Logger.Info("Artificially redirecting");
                    Container.RoamControl.Redirect.Fire();
                    break;
                }
                case "director":
                {
                    Container.DirectorSwitch.Enabled.OnNext(!Container.DirectorSwitch.Enabled.Value);
                    Logger.Info("Turned directors {0}", Container.DirectorSwitch.Enabled.Value ? "on" : "off");
                    break;
                }
                case "seeker":
                {
                    Logger.Info("Artificially spawning a seeker squad");
                    Container.SeekerGroupSpawner.Spawn();
                    break;
                }
                case "runner":
                {
                    Logger.Info("Artificially spawning a runner");
                    Container.SingleRunnerSpawner.Spawn(nearPlayer: true);
                    break;
                }
                case "crazy":
                {
                    Logger.Info("Artificially spawning a crazy");
                    Container.CrazySpawner.Spawn(nearPlayer: true);
                    break;
                }
                case "wipe":
                {
                    Logger.Info("Wiping all tracked zombies");
                    Container.ZombieGroupManager.DestroyAll();
                    if (paramList.Count > 1 && paramList[1].EqualsCaseInsensitive("all"))
                    {
                        Logger.Info("Wiping all ambient zombies");
                        Container.Ambient.DestroyAll();
                    }
                    
                    Container.ZombieGroupManager.CleanGroups();
                    break;
                }
                case "players":
                {
                    Container.PlayerZoneManager.Print();
                    break;
                }
                case "ambient":
                {
                    if (paramList.Count > 1
                        && paramList[1].EqualsCaseInsensitive("allow"))
                    {
                        Container.Ambient.AllowAmbient = !Container.Ambient.AllowAmbient;
                        Logger.Info("Turning ambient zombies {0}", Container.Ambient.AllowAmbient ? "on" : "off");
                        return;
                    }
                    Logger.Info("Spawning some ambient zombies");
                    Container.AmbientSpawner.Spawn();
                    break;
                }
                default:
                    break;
            }
        }
    }
}