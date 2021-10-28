using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VolatileHordes.Utility;

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
                    else
                    {
                        Logger.Info("Need to supply desired horde size");
                    }
                    break;
                }
                case "redirect":
                {
                    Logger.Info("Artificially redirecting");
                    Container.RoamControl.Redirect.Fire();
                    break;
                }
                case "logging":
                {
                    if (paramList.Count > 1 && Enum.TryParse<LogLevel>(paramList[1], out var logLevel))
                    {
                        Logger.Info("Setting log level to {0}", logLevel);
                        Logger.Level = logLevel;
                    }
                    else
                    {
                        Logger.Info("Need to supply desired log level");
                    }
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
                    await Container.SeekerGroupSpawner.Spawn();
                    break;
                }
                case "runner":
                {
                    Logger.Info("Artificially spawning a runner");
                    await Container.SingleRunnerSpawner.Spawn(nearPlayer: true);
                    break;
                }
                case "crazy":
                {
                    Logger.Info("Artificially spawning a crazy");
                    await Container.CrazySpawner.Spawn(nearPlayer: true);
                    break;
                }
                case "wipe":
                {
                    Logger.Info("Wiping all tracked zombies");
                    Container.ZombieGroupManager.DestroyNormal();
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
                case "destroy":
                {
                    if (paramList.Count > 1 && ushort.TryParse(paramList[1], out var size))
                    {
                        Logger.Info("Artificially destroying size {0}", size);
                        Container.LimitManager.PrintZombieStats();
                        await Container.LimitManager.Destroy(size);
                        Logger.Info("Finished artificially destroying size {0}", size);
                        Container.LimitManager.PrintZombieStats();
                    }
                    else
                    {
                        Logger.Info("Need to supply amount to destroy");
                    }
                    break;
                }
                case "ambient":
                {
                    if (paramList.Count > 1
                        && paramList[1].EqualsCaseInsensitive("enable"))
                    {
                        Container.Ambient.AllowAmbient = !Container.Ambient.AllowAmbient;
                        Logger.Info("Turning ambient zombies {0}", Container.Ambient.AllowAmbient ? "on" : "off");
                        return;
                    }
                    Logger.Info("Spawning some ambient zombies");
                    await Container.AmbientSpawner.Spawn();
                    break;
                }
                case "allocation":
                {
                    if (paramList.Count <= 2
                        || !paramList[1].EqualsCaseInsensitive("set"))
                    {
                        return;
                    }

                    if (float.TryParse(paramList[2], out var amount))
                    {
                        var perc = Percent.FactoryPutInRange(amount);
                        Logger.Info("Setting allocation buckets to {0}", perc);
                        for (int x = 0; x < Container.AllocationBuckets.Width; x++)
                        {
                            for (int y = 0; y < Container.AllocationBuckets.Height; y++)
                            {
                                Container.AllocationBuckets[x, y] = perc;
                            }
                        }
                    }
                    break;
                }
                default:
                    break;
            }
        }
    }
}