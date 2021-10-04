using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VolatileHordes.Settings.User.Control;
using VolatileHordes.Settings.User.Director;
using VolatileHordes.Spawning.WanderingHordes;

namespace VolatileHordes.Settings.User
{
    public class UserSettings
    {
        private static readonly JsonSerializerSettings JsonSettings;

        static UserSettings()
        {
            JsonSettings = new JsonSerializerSettings();
            JsonSettings.Converters.Add(new StringEnumConverter());
        }
            
        public static string SettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "Mods", Constants.ModName, $"{Constants.ModName}.json");

        public ControlSettings Control { get; set; } = new();

        public NoiseSettings Noise { get; set; } = new();

        public DirectorSettings Director { get; set; } = new();

        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        public static UserSettings Load()
        {
            var readIn = ReadIn();
            Logger.Level = readIn.LogLevel;
            return readIn;
        }

        private static UserSettings ReadIn()
        {
            if (File.Exists(SettingsPath))
            {
                var readIn = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SettingsPath), JsonSettings);
                return readIn ?? new();
            }
            else
            {
                var instance = new UserSettings();
                File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(instance, Formatting.Indented, JsonSettings));
                return instance;
            }
        }

        public WanderingHordeSettings WanderingHordeSettings = new();
    }
}