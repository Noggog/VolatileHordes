using System.IO;
using Newtonsoft.Json;

namespace VolatileHordes.GUI.Services
{
    public class SaveSettings 
    {
        private readonly ISettingsTarget[] _targets;

        public SaveSettings(ISettingsTarget[] targets)
        {
            _targets = targets;
        }
        
        public void Save()
        {
            var settings = new Settings();
            foreach (var target in _targets)
            {
                target.Save(settings);
            }
            
            File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}