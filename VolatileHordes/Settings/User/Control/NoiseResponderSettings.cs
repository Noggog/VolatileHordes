namespace VolatileHordes.Settings.User.Control
{
    public class NoiseResponderSettings
    {
        public ushort Radius { get; set; } = 350;
        public byte InvestigationDistanceMin { get; set; } = 6;
        public byte InvestigationDistanceMax { get; set; } = 25;
        public float NoiseLostPerTwoSeconds { get; set; } = 0.1f;
    }
}