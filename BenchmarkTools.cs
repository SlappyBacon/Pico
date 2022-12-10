using System;

namespace Pico.Benchmark
{
	public static class BenchmarkTools
    {
        /// <summary>
        /// Returns the average amount of
        /// seconds it takes to perform an
        /// action 'count'  amount of times.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static double Benchmark(Action method, int count = 1)
        {
            if (count < 1) count = 1;
            double time = 0;
            DateTime start;
            DateTime end;
            for (int i = 0; i < count;)
            {
                start = DateTime.Now;
                method.Invoke();
                end = DateTime.Now;
                time += (end - start).TotalSeconds;
                i++;
            }
            time /= count;
            return time;
        }
    }
}