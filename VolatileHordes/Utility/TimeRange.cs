using System;

namespace VolatileHordes
{
    public struct TimeRange : IEquatable<TimeRange>
    {
        public readonly TimeSpan From;
        public readonly TimeSpan To;

        public TimeSpan Diff => To - From;

        public TimeRange(TimeSpan t1, TimeSpan t2)
        {
            if (t1 < t2)
            {
                From = t1;
                To = t2;
            }
            else
            {
                From = t2;
                To = t1;
            }
        }

        public bool Equals(TimeRange other)
        {
            return From.Equals(other.From) && To.Equals(other.To);
        }

        public override bool Equals(object? obj)
        {
            return obj is TimeRange other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (From.GetHashCode() * 397) ^ To.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{From}-{To}";
        }
    }
}