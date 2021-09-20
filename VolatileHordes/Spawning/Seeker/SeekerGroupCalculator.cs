using VolatileHordes.Randomization;
using VolatileHordes.Zones;

namespace VolatileHordes.Spawning.Seeker
{
    public class SeekerGroupCalculator
    {
        private readonly RandomSource _randomSource;

        public SeekerGroupCalculator(
            RandomSource randomSource)
        {
            _randomSource = randomSource;
        }
        
        public int GetSeekerGroupSize(PlayerZone player)
        {
            // ToDo
            // Improve logic
            return _randomSource.Get(1, 3);
        }
    }
}