namespace VolatileHordes.Settings.User.Control
{
    public class NoiseResponderSettings
    {
        public byte Radius { get; set; } = 100;
        public byte InvestigationDistanceMin { get; set; } = 6;
        public byte InvestigationDistanceMax { get; set; } = 25;
        public float NoiseLostPerTwoSeconds { get; set; } = 0.1f;
    }
}