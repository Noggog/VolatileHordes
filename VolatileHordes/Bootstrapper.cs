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
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(Container.PlayerZoneManager.PlayerSpawnedInWorld);
            ModEvents.PlayerDisconnected.RegisterHandler(Container.PlayerZoneManager.PlayerDisconnected);
        }

        static void GameStarted()
        {
            Logger.Info($"Game started");
            Container.Biome.Init();
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