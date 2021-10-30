using System;
using System.Collections.Generic;

namespace VolatileHordes.Core.Extensions
{
    public static class DictionaryExt
    {
        public static TValue TryCreate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> create)
        {
            if (!dict.TryGetValue(key, out var val))
            {
                val = create(key);
                dict[key] = val;
            }

            return val;
        }
        
        public static TValue GetAndRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            var val = dict[key];
            dict.Remove(key);
            return val;
        }
    }
}