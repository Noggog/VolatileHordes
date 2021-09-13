using System.Collections.Generic;

namespace VolatileHordes
{
    public static class ListExt
    {
        public static void AddIfNotNull<T>(this IList<T> list, T? item)
        {
            if (item == null) return;
            list.Add(item);
        }
    }
    
    public static class ListExt<T>
    {
        public static readonly T[] Empty = new T[0];
    }
}