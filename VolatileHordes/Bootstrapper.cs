using System;

namespace VolatileHordes
{
    public class Bootstrapper : IModApi
    {
        public void InitMod()
        {
            ModEvents.GameStartDone.RegisterHandler(GameStarted);
            ModEvents.GameUpdate.RegisterHandler(GameUpdate);
            // ModEvents.GameShutdown.RegisterHandler(GameShutdown);
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(PlayerZoneManager.Instance.PlayerSpawnedInWorld);
            ModEvents.PlayerDisconnected.RegisterHandler(PlayerZoneManager.Instance.PlayerDisconnected);
        }

        static void GameStarted()
        {
            Logger.Info($"Game started");
            HordeManager.Instance.Init();
            BiomeData.Instance.Init();
        }

        static void GameUpdate()
        {
            try
            {
                TimeManager.Instance.Update();
            }
            catch (Exception e)
            {
                Logger.Error($"Error in API.GameUpdate: {e.Message}:\n{e.StackTrace}");
            }
        }
    }
}