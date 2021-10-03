using System;
using System.Collections.Generic;

namespace VolatileHordes
{
    public struct ValueTuple<T1, T2> : IEquatable<ValueTuple<T1, T2>>
    {
        public readonly T1 Item1;
        public readonly T2 Item2;

        public ValueTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public bool Equals(ValueTuple<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                   && EqualityComparer<T2>.Default.Equals(Item2, other.Item2);
        }

        public override bool Equals(object? obj)
        {
            return obj is ValueTuple<T1, T2> other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T1>.Default.GetHashCode(Item1) * 397) 
                       ^ EqualityComparer<T2>.Default.GetHashCode(Item2);
            }
        }
    }
}