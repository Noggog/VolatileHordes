using System;
using VolatileHordes.Randomization;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordeCalculator
    {
        static double Interpolate(float min, float max, float prog)
        {
            var diff = max - min;
            return min + (diff * prog);
        }
        
        public int GetHordeSize(
            IWanderingHordeSettingsGetter settings,
            RandomSource rand,
            ref int noHordeCounter,
            int gamestage)
        {
            float percentThroughGeneration = 1.0f * gamestage / settings.UpperGamestage;
            
            var percentLargeHorde = Interpolate(settings.PercentLargeHordeStart, settings.PercentLargeHordeEnd, percentThroughGeneration);
            percentLargeHorde += noHordeCounter * settings.PercentAddedWhenNoHorde;
            var large = rand.Random.NextDouble() <= percentLargeHorde;
            int lower, upper;
            if (large)
            {
                noHordeCounter = 0;
                lower = settings.LowerHorde;
                upper = settings.UpperHorde;
            }
            else
            {
                noHordeCounter++;
                lower = settings.LowerTrickle;
                upper = settings.UpperTrickle;
            }
            var variation = settings.Variation * rand.Random.NextDouble() * (rand.Random.Next(2) == 1 ? 1 : -1);
            var dResult = Interpolate(lower, upper, percentThroughGeneration);
            dResult += dResult * variation;
            return (int)Math.Round(dResult);
        }
    }
}