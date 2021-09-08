namespace VolatileHordes.Spawning.WanderingHordes
{
    public interface IWanderingHordeSettingsGetter
    {
        public bool Enabled { get; }
        int NumToGenerate { get; }
        int LowerTrickle { get; }
        int UpperTrickle { get; }
        int LowerHorde { get; }
        int UpperHorde { get; }
        int UpperGamestage { get; }
        float Variation { get; }
        float PercentLargeHordeStart { get; }
        float PercentLargeHordeEnd { get; }
        float PercentAddedWhenNoHorde { get; }
    }

    public class WanderingHordeSettings : IWanderingHordeSettingsGetter
    {
        public bool Enabled { get; set; } = true;
        public int NumToGenerate { get; set; } = 150;
        public int LowerTrickle { get; set; } = 6;
        public int UpperTrickle { get; set; } = 15;
        public int LowerHorde { get; set; } = 15;
        public int UpperHorde { get; set; } = 50;
        public int UpperGamestage { get; set; } = 150;
        public float Variation { get; set; } = 0.2f;
        public float PercentLargeHordeStart { get; set; } = 0.35f;
        public float PercentLargeHordeEnd { get; set; } = 0.6f;
        public float PercentAddedWhenNoHorde { get; set; } = 0.1f;
    }
}