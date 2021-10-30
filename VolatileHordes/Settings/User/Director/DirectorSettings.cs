namespace VolatileHordes.Settings.User.Director
{
    public class DirectorSettings
    {
        public bool Enabled { get; set; } = true;
        public WanderInHordeDirectorSettings WanderInHorde { get; set; } = new();
    }
}