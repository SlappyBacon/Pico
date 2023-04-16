using System;
using System.Threading.Tasks;

namespace Pico.Benchmark
{
	public static class BenchmarkTools
    {
        public static async Task<TimeSpan> BenchmarkAsync(Task asyncTask)
        {
            DateTime start = DateTime.Now;
            await asyncTask;
            return DateTime.Now - start;
        }
    }
}