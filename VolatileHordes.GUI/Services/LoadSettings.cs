using System.IO;
using Newtonsoft.Json;

namespace VolatileHordes.GUI.Services
{
    public class LoadSettings : IStartupTask
    {
        private readonly ISettingsTarget[] _targets;

        public LoadSettings(ISettingsTarget[] targets)
        {
            _targets = targets;
        }
        
        public void Start()
        {
            var settings = new Settings();
            if (File.Exists("settings.json"))
            {
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.json"))!;
            }

            foreach (var target in _targets)
            {
                target.Load(settings);
            }
        }
    }
}