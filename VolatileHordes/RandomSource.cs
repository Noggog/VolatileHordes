using System;

namespace VolatileHordes
{
    public class RandomSource
    {
	    public static readonly RandomSource Instance = new();
	    
		private Random _rand = new Random();

		public RandomSource()
		{
		}

		// min:0, max:int.MaxValue
		public int Get()
		{
			return _rand.Next();
		}

		// min:0, max:exclusive
		public int Get(int max)
		{
			return _rand.Next(max);
		}

		// min:inclusive, max:exclusive
		public int Get(int min, int max)
		{
			return _rand.Next(min, max);
		}

		// min:inclusive, max:inclusive
		public float Get(float min, float max)
		{
			return (float)(_rand.NextDouble() * (max - min) + min);
		}

		public bool Chance(float c)
		{
			if (c < 0.0f || c > 1.0f)
				throw new InvalidOperationException("Parameter range is 0.0 to 1.0 inclusive");

			return Get(0.0f, 1.0f) <= c;
		}
    }
}