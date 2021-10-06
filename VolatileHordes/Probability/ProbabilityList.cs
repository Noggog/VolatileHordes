using System.Collections.Generic;
using VolatileHordes.Containers;
using VolatileHordes.Core.Models;

namespace VolatileHordes.Probability
{
    public class ProbabilityList<T>
    {
        private double _runningTotal;
        private readonly SortingListDictionary<double, T> _items = new();

        public void Add(T item, UDouble weight)
        {
            _runningTotal += weight.Value;
            _items[_runningTotal] = item;
        }

        public T Get(RandomSource random)
        {
            if (_items.Count == 0)
            {
                throw new KeyNotFoundException("No items on probability list");
            }
            var roll = random.NextDouble(_runningTotal);
            if (!_items.TryGetInDirection(roll, higher: false, out var result))
            {
                throw new KeyNotFoundException("Could not find item for roll");
            }

            return result.Value;
        }
    }
}