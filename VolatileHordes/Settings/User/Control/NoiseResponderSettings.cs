namespace VolatileHordes.Settings.User.Control
{
    public class NoiseResponderSettings
    {
        public byte Radius { get; set; } = 100;
        public float MaxBaseChance { get; set; } = 0.5f;
        public byte InvestigationDistance { get; set; } = 10;
        public float MaxVolumeMultiplier { get; set; } = 4f;
    }
}