using System.Collections.Generic;
using VolatileHordes.Randomization;

namespace VolatileHordes
{
    public static class ListExt
    {
        public static void AddIfNotNull<T>(this IList<T> list, T? item)
        {
            if (item == null) return;
            list.Add(item);
        }

        public static IEnumerable<T> EnumerateAllFromIndex<T>(this IReadOnlyList<T> list, int startIndex)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var index = (startIndex + i) % list.Count;
                yield return list[index];
            }
        }

        public static IEnumerable<T> EnumerateFromRandomIndex<T>(this IReadOnlyList<T> list, RandomSource randomSource)
        {
            return EnumerateAllFromIndex(list, randomSource.Random.Next(list.Count - 1));
        }
    }
    
    public static class ListExt<T>
    {
        public static readonly T[] Empty = new T[0];
    }
}