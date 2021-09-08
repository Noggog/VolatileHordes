using System.IO;
using Newtonsoft.Json;
using VolatileHordes.Spawning.WanderingHordes;

namespace VolatileHordes
{
    public class Settings
    {
        public static string SettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "Mods", Constants.ModName, $"{Constants.ModName}.json");

        public static Settings Instance { get; private set; } = null!;

        public static void Load()
        {
            if (File.Exists(SettingsPath))
            {
                var readIn = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsPath));
                Instance = readIn ?? new();
            }
            else
            {
                Instance = new();
                File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(Instance, Formatting.Indented));
            }
        }

        public WanderingHordeSettings WanderingHordeSettings = new();
    }
}