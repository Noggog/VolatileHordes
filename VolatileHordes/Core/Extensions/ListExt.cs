using System.Collections.Generic;
using VolatileHordes.Probability;

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
        
        public static int BinarySearch<T>(this IReadOnlyList<T> list, T value)
        {
            if (list.Count == 0) return ~0;
            var comp = Comparer<T>.Default;
            int low = 0;
            int high = list.Count - 1;
            while (low < high)
            {
                var index = low + (high - low) / 2;
                var result = comp.Compare(list[index], value);
                if (result == 0)
                {
                    return index;
                }
                else if (result < 0)
                {
                    low = index + 1;
                }
                else
                {
                    high = index - 1;
                }
            }
            var c = comp.Compare(list[low], value);
            if (c < 0)
            {
                low++;
            }
            else if (c == 0)
            {
                return low;
            }
            return ~low;
        }

        // IList does not implement IReadOnlyList
        public static int BinarySearch<T>(this IList<T> list, T value)
        {
            if (list.Count == 0) return ~0;
            var comp = Comparer<T>.Default;
            int low = 0;
            int high = list.Count - 1;
            while (low < high)
            {
                var index = low + (high - low) / 2;
                var result = comp.Compare(list[index], value);
                if (result == 0)
                {
                    return index;
                }
                else if (result < 0)
                {
                    low = index + 1;
                }
                else
                {
                    high = index - 1;
                }
            }
            var c = comp.Compare(list[low], value);
            if (c < 0)
            {
                low++;
            }
            else if (c == 0)
            {
                return low;
            }
            return ~low;
        }

        // To avoid compiler confusion
        public static int BinarySearch<T>(this List<T> list, T value)
        {
            return BinarySearch<T>((IReadOnlyList<T>)list, value);
        }
    }
    
    public static class ListExt<T>
    {
        public static readonly T[] Empty = new T[0];
    }
}