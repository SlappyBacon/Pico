using System;

namespace Pico.Benchmark
{
	public static class BenchmarkTools
    {
        /// <summary>
        /// Returns the amount of seconds
        /// it takes to perform an action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static double Benchmark(Action action)
        {
            DateTime start = DateTime.Now;
            action();
            return (DateTime.Now - start).TotalSeconds;
        }

        public static double Benchmark(int count, Action action)
        {
            double time = 0;
            for (int i = 0; i < count;)
            {
                time += Benchmark(action);
                i++;
            }
            time /= count;
            return time;
        }
    }
}