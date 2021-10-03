namespace VolatileHordes.Settings.User
{
    public record NoiseItem(string Name, byte Volume)
    {
        public string Name { get; } = Name;
        public byte Volume { get; } = Volume;
    }
}