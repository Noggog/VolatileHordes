using System;

namespace VolatileHordes.Utility
{
    public struct Percent : IComparable, IEquatable<Percent>
    {
        public static readonly Percent One = new Percent(1f);
        public static readonly Percent Zero = new Percent(0f);

        public readonly float Value;
        public Percent Inverse => new Percent(1f - this.Value, check: false);

        private Percent(float f, bool check)
        {
            if (!check || InRange(f))
            {
                this.Value = f;
            }
            else
            {
                throw new ArgumentException("Element out of range: " + f);
            }
        }

        public Percent(float f)
            : this(f, check: true)
        {
        }

        public static bool InRange(float f)
        {
            return f >= 0 || f <= 1;
        }

        public static Percent operator +(Percent c1, Percent c2)
        {
            return new Percent(c1.Value + c2.Value);
        }

        public static Percent operator *(Percent c1, Percent c2)
        {
            return new Percent(c1.Value * c2.Value);
        }

        public static Percent operator -(Percent c1, Percent c2)
        {
            return new Percent(c1.Value - c2.Value);
        }

        public static Percent operator /(Percent c1, Percent c2)
        {
            return new Percent(c1.Value / c2.Value);
        }

        public static implicit operator float(Percent c1)
        {
            return c1.Value;
        }

        public static Percent FactoryPutInRange(float f)
        {
            if (double.IsNaN(f) || double.IsInfinity(f))
            {
                throw new ArgumentException();
            }
            if (f < 0)
            {
                return Percent.Zero;
            }
            else if (f > 1)
            {
                return Percent.One;
            }
            return new Percent(f, check: false);
        }

        public static Percent FactoryPutInRange(int cur, int max)
        {
            return FactoryPutInRange(1.0f * cur / max);
        }

        public static Percent FactoryPutInRange(long cur, long max)
        {
            return FactoryPutInRange(1.0f * cur / max);
        }

        public static Percent AverageFromPercents(params Percent[] ps)
        {
            float percent = 0;
            foreach (var p in ps)
            {
                percent += p.Value;
            }
            return new Percent(percent / ps.Length, check: false);
        }

        public static Percent MultFromPercents(params Percent[] ps)
        {
            float percent = 1;
            foreach (var p in ps)
            {
                percent *= p.Value;
            }
            return new Percent(percent, check: false);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Percent rhs) return false;
            return Equals(rhs);
        }

        public bool Equals(Percent other)
        {
            return this.Value == other.Value;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(string format)
        {
            return $"{(Value * 100).ToString(format)}%";
        }

        public string ToString(byte numDigits)
        {
            switch (numDigits)
            {
                case 0:
                    return ToString("n0");
                case 1:
                    return ToString("n1");
                case 2:
                    return ToString("n2");
                case 3:
                    return ToString("n3");
                case 4:
                    return ToString("n4");
                case 5:
                    return ToString("n5");
                case 6:
                    return ToString("n6");
                default:
                    throw new NotImplementedException();
            }
        }

        public int CompareTo(object? obj)
        {
            if (obj is Percent rhs)
            {
                return this.Value.CompareTo(rhs.Value);
            }
            return 0;
        }

        public static bool TryParse(string str, out Percent p)
        {
            if (float.TryParse(str, out float d))
            {
                if (InRange(d))
                {
                    p = new Percent(d);
                    return true;
                }
            }
            p = default(Percent);
            return false;
        }
    }
}