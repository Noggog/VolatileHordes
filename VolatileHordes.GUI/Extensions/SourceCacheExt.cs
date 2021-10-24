using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using Noggog;

namespace VolatileHordes.GUI.Extensions
{
    public static class SourceCacheExt
    {
        public static void AbsorbIn<TList, TKey, TObject>(
            this ISourceCache<TList, TKey> cache,
            IEnumerable<TObject> objs,
            Func<TObject, TKey> keySelector,
            Func<TKey, TList> creator,
            Action<TList, TObject> update)
            where TKey : notnull
        {
            var notSeen = new HashSet<TKey>(objs.Select(keySelector));
            foreach (var o in objs)
            {
                var k = keySelector(o);
                notSeen.Remove(k);
                if (!cache.TryGetValue(k, out var listItem))
                {
                    listItem = creator(k);
                    cache.AddOrUpdate(listItem);
                }

                update(listItem, o);
            }

            foreach (var i in notSeen)
            {
                cache.Remove(i);
            }
        }
    }
}