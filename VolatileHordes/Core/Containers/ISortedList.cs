using System.Collections;
using System.Collections.Generic;
using VolatileHordes.Core.Models;

namespace VolatileHordes.Containers
{
    public interface ISortedListGetter<T> : IReadOnlyList<T>
    {
        bool TryGetIndexInDirection(
            T item,
            bool higher,
            out int result);

        bool TryGetValueInDirection(
            T item,
            bool higher,
            out T result);

        bool TryGetInDirection(
            T item,
            bool higher,
            out KeyValuePair<int, T> result);

        bool TryGetEncapsulatedIndices(
            T lowerKey,
            T higherKey,
            out RangeInt32 result);

        bool TryGetEncapsulatedValues(
            T lowerKey,
            T higherKey,
            out IEnumerable<KeyValuePair<int, T>> result);
    }

    public interface ISortedList<T> : ISortedListGetter<T>, IList<T>, ICollection
    {
        bool Add(T item, bool replaceIfMatch);
    }
}