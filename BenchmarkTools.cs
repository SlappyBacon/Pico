using System;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Pico.Benchmark
{
	public static class BenchmarkTools
    {
        public static double BenchmarkCompareAverage(Action oldAction, Action newAction, int count, params object?[]? args)
        {
            var oldAverage = BenchmarkAverage(oldAction, count, args);
            var newAverage = BenchmarkAverage(newAction, count, args);
            return newAverage.TotalMilliseconds / oldAverage.TotalMilliseconds;
        }
        public static double BenchmarkCompare(Action oldAction, Action newAction, params object?[]? args)
        {
            var oldTime = Benchmark(oldAction, args);
            var newTime = Benchmark(newAction, args);
            return newTime.TotalMilliseconds / oldTime.TotalMilliseconds;
        }


        /// <summary>
        /// Benchmark an action.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="args"></param>
        /// <returns>The average time it takes to perform as TimeSpan.</returns>
        public static TimeSpan BenchmarkAverage(Action action, int count, params object?[]? args)
        {
            TimeSpan result = TimeSpan.Zero;
            for (int i = 0; i < count; i++)
            {
                result += Benchmark(action, args);
            }
            result /= count;
            return result;
        }
        /// <summary>
        /// Benchmark an action.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="args"></param>
        /// <returns>The times it takes to perform as TimeSpan[].</returns>
        public static TimeSpan[] Benchmark(Action action, int count, params object?[]? args)
        {
            TimeSpan[] result = new TimeSpan[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = Benchmark(action, args);
            }
            return result;
        }
        /// <summary>
        /// Benchmark an action.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="args"></param>
        /// <returns>The time it takes to perform as TimeSpan.</returns>
        public static TimeSpan Benchmark(Action action, params object?[]? args)
        {
            DateTime start = DateTime.Now;
            action?.DynamicInvoke(args);
            return DateTime.Now - start;
        }







        /// <summary>
        /// Benchmark a delegate.
        /// </summary>
        /// <param name="del"></param>
        /// <param name="args"></param>
        /// <returns>The average time it takes to perform as TimeSpan.</returns>
        public static TimeSpan BenchmarkAverage(Delegate del, int count, params object?[]? args)
        {
            TimeSpan result = TimeSpan.Zero;
            for (int i = 0; i < count; i++)
            {
                result += Benchmark(del, args);
            }
            result /= count;
            return result;
        }
        /// <summary>
        /// Benchmark a delegate.
        /// </summary>
        /// <param name="del"></param>
        /// <param name="args"></param>
        /// <returns>The times it takes to perform as TimeSpan[].</returns>
        public static TimeSpan[] Benchmark(Delegate del, int count, params object?[]? args)
        {
            TimeSpan[] result = new TimeSpan[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = Benchmark(del, args);
            }
            return result;
        }
        /// <summary>
        /// Benchmark a delegate.
        /// </summary>
        /// <param name="del"></param>
        /// <param name="args"></param>
        /// <returns>The time it takes to perform as TimeSpan.</returns>
        public static TimeSpan Benchmark(Delegate del, params object?[]? args)
        {
            DateTime start = DateTime.Now;
            del?.DynamicInvoke(args);
            return DateTime.Now - start;
        }





        /// <summary>
        /// Benchmark a task.
        /// </summary>
        /// <param name="task">The time it takes to perform as TimeSpan.</param>
        /// <returns></returns>
        public static TimeSpan Benchmark(Task task)
        {
            DateTime start = DateTime.Now;
            task.Start();
            task.Wait();
            return DateTime.Now - start;
        }






        /// <summary>
        /// Benchmark a task.
        /// </summary>
        /// <param name="task">The time it takes to perform as TimeSpan.</param>
        /// <returns></returns>
        public static async Task<TimeSpan> BenchmarkAsync(Task task)
        {
            DateTime start = DateTime.Now;
            await task;
            return DateTime.Now - start;
        }
    }
}