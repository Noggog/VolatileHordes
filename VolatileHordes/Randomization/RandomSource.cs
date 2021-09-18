using System;
using VolatileHordes.Utility;

namespace VolatileHordes.Randomization
{
    public class RandomSource
    {
		public Random Random { get; } = new();

		// min:0, max:int.MaxValue
		public int Get()
		{
			return Random.Next();
		}

		// min:0, max:exclusive
		public int Get(int max)
		{
			return Random.Next(max);
		}

		// min:inclusive, max:exclusive
		public int Get(int min, int max)
		{
			return Random.Next(min, max);
		}

		// min:inclusive, max:inclusive
		public float Get(float min, float max)
		{
			return (float)(Random.NextDouble() * (max - min) + min);
		}

		public bool NextBool() => Random.Next(0, 1) == 0;

		public bool Chance(float c)
		{
			if (c < 0.0f || c > 1.0f)
				throw new InvalidOperationException("Parameter range is 0.0 to 1.0 inclusive");

			return Get(0.0f, 1.0f) <= c;
		}

		public double NextDouble() => Random.NextDouble();

		public double NextDouble(double max) => Random.NextDouble() * max;

		public bool NextChance(Percent chance) => Random.NextDouble() <= chance.Value;

		public double NextSignedDouble()
		{
			var rand = NextDouble();
			if (NextBool())
			{
				return rand * -1;
			}

			return rand;
		}

		public TimeSpan GetRandomTime(TimeRange range)
		{
			var rand = NextDouble();
			var deviation = (long)(rand * range.Diff.Ticks);
			return range.From.AddTicks(deviation);
		}
    }
}