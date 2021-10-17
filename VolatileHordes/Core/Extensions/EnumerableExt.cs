using System;
using System.Collections.Generic;
using System.Linq;

namespace VolatileHordes
{
    public static class EnumerableExt
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            return new HashSet<T>(items);
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> e)
            where T : class
        {
            return e.Where(i => i != null)!;
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> e)
            where T : struct
        {
            return e.Where(i => i.HasValue)
                .Select(i => i!.Value);
        }

        public static String ToLogString<T>(this IEnumerable<T?> e)
        {
            return $"[{String.Join(", ", e)}]";
        }
    }
}