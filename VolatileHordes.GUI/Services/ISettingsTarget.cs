namespace VolatileHordes.GUI.Services
{
    public interface ISettingsTarget
    {
        void Load(Settings settings);
        void Save(Settings settings);
    }
}