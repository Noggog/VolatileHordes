﻿using System.Collections.Generic;
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
                case "crazy":
                {
                    Logger.Info("Artificially spawning a crazy");
                    Container.CrazyDirector.Spawn(nearPlayer: true);
                    break;
                }
                case "wipe":
                {
                    Logger.Info("Wiping all tracked zombies");
                    Container.GroupManager.DestroyAll();
                    if (paramList.Count > 1 && paramList[1].EqualsCaseInsensitive("all"))
                    {
                        Logger.Info("Wiping all ambient zombies");
                        Container.Ambient.DestroyAll();
                    }
                    break;
                }
                case "players":
                {
                    Container.PlayerZoneManager.Print();
                    break;
                }
                case "ambient":
                {
                    Container.AmbientSpawner.Spawn();
                    break;
                }
                default:
                    break;
            }
        }
    }
}