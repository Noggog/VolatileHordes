namespace VolatileHordes.GUI
{
    public class Settings
    {
        public string Ip { get; set; } = "127.0.0.1";
        public ushort Port { get; set; } = 13633;
        public bool DrawGroupTargets { get; set; } = false;
        public bool DrawTargets { get; set; } = true;
    }
}