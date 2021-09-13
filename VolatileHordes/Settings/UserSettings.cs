using System.IO;
using Newtonsoft.Json;
using VolatileHordes.Spawning.WanderingHordes;

namespace VolatileHordes
{
    public class UserSettings
    {
        public static string SettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "Mods", Constants.ModName, $"{Constants.ModName}.json");

        public static UserSettings Load()
        {
            if (File.Exists(SettingsPath))
            {
                var readIn = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SettingsPath));
                return readIn ?? new();
            }
            else
            {
                var instance = new UserSettings();
                File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(instance, Formatting.Indented));
                return instance;
            }
        }

        public WanderingHordeSettings WanderingHordeSettings = new();
    }
}