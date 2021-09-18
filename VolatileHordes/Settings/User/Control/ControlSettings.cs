namespace VolatileHordes.Settings.User.Control
{
    public class ControlSettings
    {
        public RoamControlSettings FidgetRoam { get; set; } = new()
        {
            MinSeconds = 15,
            MaxSeconds = 30,
            Range = 5
        };
        public RoamControlSettings FarRoam { get; set; } = new()
        {
            MinSeconds = 60,
            MaxSeconds = 600,
            Range = 80
        };
    }
}