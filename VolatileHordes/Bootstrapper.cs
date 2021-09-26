using System;
using System.Reactive;
using System.Reactive.Subjects;
using HarmonyLib;

namespace VolatileHordes
{
    public class Bootstrapper : IModApi
    {
        public void InitMod()
        {
            ModEvents.GameStartDone.RegisterHandler(StartGame);
            ModEvents.GameUpdate.RegisterHandler(GameUpdate);
            ModEvents.GameShutdown.RegisterHandler(GameShutdown);
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(Container.PlayerZoneManager.PlayerSpawnedInWorld);
            ModEvents.PlayerDisconnected.RegisterHandler(Container.PlayerZoneManager.PlayerDisconnected);
        }

        private static ReplaySubject<Unit> _gameStarted = new();
        public static IObservable<Unit> GameStarted => _gameStarted;

        static void StartGame()
        {
            Logger.Info($"Game started");
            Settings.World.WorldState.Load();
            Container.Biome.Init();
            _gameStarted.OnNext(Unit.Default);

            InstallHooks();
        }
        
        static void InstallHooks()
        {
            var harmony = new Harmony($"{Constants.ModName}.Hooks");
            harmony.PatchAll();
        }

        static void GameUpdate()
        {
            try
            {
                Container.Time.Update();
            }
            catch (Exception e)
            {
                Logger.Error($"Error in API.GameUpdate: {e.Message}:\n{e.StackTrace}");
            }
        }

        static void GameShutdown()
        {
            Logger.Info($"Game shutdown");
            Settings.World.WorldState.Save();
        }
    }
}