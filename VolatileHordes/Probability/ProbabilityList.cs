using System.Collections.Generic;
using VolatileHordes.Containers;
using VolatileHordes.Core.Models;

namespace VolatileHordes.Probability
{
    public class ProbabilityList<T>
    {
        public UDouble TotalWeight { get; private set; }
        private readonly SortingListDictionary<double, T> _items = new();

        public void Add(T item, UDouble weight)
        {
            TotalWeight = new UDouble(weight.Value + TotalWeight.Value);
            _items[TotalWeight.Value] = item;
        }

        public T Get(RandomSource random)
        {
            if (_items.Count == 0)
            {
                throw new KeyNotFoundException("No items on probability list");
            }
            var roll = random.NextDouble(TotalWeight.Value);
            return GetAt(new UDouble(roll));
        }

        public T GetAt(UDouble roll)
        {
            if (!_items.TryGetInDirection(roll.Value, higher: true, out var result))
            {
                throw new KeyNotFoundException("Could not find item for roll");
            }

            return result.Value;
        }
    }
}