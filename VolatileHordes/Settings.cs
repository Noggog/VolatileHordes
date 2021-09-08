using System.IO;

namespace VolatileHordes
{
    public class Settings
    {
        public static string ModPath = Path.Combine(Directory.GetCurrentDirectory(), "Mods", Constants.ModName, $"{Constants.ModName}.xml");
    }
}