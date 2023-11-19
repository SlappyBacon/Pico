using System;
using System.Threading.Tasks;

namespace Pico.Benchmark
{
	public static class BenchmarkTools
    {
        
        public static TimeSpan Benchmark(Action action, params object?[]? args)
        {
            DateTime start = DateTime.Now;
            action?.DynamicInvoke(args);
            return DateTime.Now - start;
        }
        public static TimeSpan Benchmark(Delegate del, params object?[]? args)
        {
            DateTime start = DateTime.Now;
            del?.DynamicInvoke(args);
            return DateTime.Now - start;
        }
        public static TimeSpan Benchmark(Task task)
        {
            DateTime start = DateTime.Now;
            task.Start();
            task.Wait();
            return DateTime.Now - start;
        }
        public static async Task<TimeSpan> BenchmarkAsync(Task task)
        {
            DateTime start = DateTime.Now;
            await task;
            return DateTime.Now - start;
        }
    }
}