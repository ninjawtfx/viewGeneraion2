using System;
using System.Threading;

namespace ViewGeneration
{
	public static class Randomize
	{
		static int seed = Environment.TickCount;

		static readonly ThreadLocal<Random> random =
			new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

		public static int Rand()
		{
			return random.Value.Next(0, 3);
		}
	}
}
