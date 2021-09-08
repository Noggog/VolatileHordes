using System.Collections.Generic;

namespace VolatileHordes.Randomization
{
    public static class RandomExt
    {
        public static T? Random<T>(this IReadOnlyList<T> list, RandomSource random)
        {
            if (list.Count == 0) return default;
            var selection = random.Random.Next(0, list.Count - 1);
            return list[selection];
        }
    }
}