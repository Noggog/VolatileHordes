using System;
using System.Reactive.Linq;

namespace MoreHordes
{
    public class HordeManager
    {
        public static readonly HordeManager Instance = new();
        
        public void Init()
        {
            TimeManager.Instance.Interval(TimeSpan.FromSeconds(30))
                .Subscribe(_ =>
                {
                    Logger.Info("Spawning a zombie");
                    var playerZone = PlayerZoneManager.Instance.GetRandom(RandomSource.Instance);
                    if (playerZone == null)
                    { 
                        Logger.Info("No player to spawn next to");
                        return;
                    }
                    ZombieCreator.Instance.CreateZombie(playerZone);
                });
        }
    }
}