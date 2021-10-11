using System;
using VolatileHordes.Probability;

namespace VolatileHordes.Spawning.WanderingHordes
{
    public class WanderingHordeCalculator
    {
        private readonly WanderingHordeSettings _settings;
        private readonly RandomSource _rand;

        public WanderingHordeCalculator(
            WanderingHordeSettings settings,
            RandomSource rand)
        {
            _settings = settings;
            _rand = rand;
        }
        
        static double Interpolate(float min, float max, float prog)
        {
            var diff = max - min;
            return min + (diff * prog);
        }
        
        public int GetHordeSize(float gameStage, ref int noHordeCounter)
        {
            float percentThroughGeneration = gameStage / _settings.UpperGamestage;
            
            var percentLargeHorde = Interpolate(_settings.PercentLargeHordeStart, _settings.PercentLargeHordeEnd, percentThroughGeneration);
            percentLargeHorde += noHordeCounter * _settings.PercentAddedWhenNoHorde;
            var large = _rand.Random.NextDouble() <= percentLargeHorde;
            int lower, upper;
            if (large)
            {
                noHordeCounter = 0;
                lower = _settings.LowerHorde;
                upper = _settings.UpperHorde;
            }
            else
            {
                noHordeCounter++;
                lower = _settings.LowerTrickle;
                upper = _settings.UpperTrickle;
            }
            var variation = _settings.Variation * _rand.Random.NextDouble() * (_rand.Random.Next(2) == 1 ? 1 : -1);
            var dResult = Interpolate(lower, upper, percentThroughGeneration);
            dResult += dResult * variation;
            return (int)Math.Round(dResult);
        }
    }
}