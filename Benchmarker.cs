namespace Pico.Benchmarker
{
    public static class Benchmarker
    {
        /*public static double BenchmarkCompareParallel(Action oldAction, Action newAction, int count, params object?[]? args)
        {
            throw new NotImplementedException();
        }*/
        public static double BenchmarkCompareAverage(Action oldAction, Action newAction, int count)
        {
            var oldAverage = BenchmarkAverage(oldAction, count);
            var newAverage = BenchmarkAverage(newAction, count);
            return newAverage.TotalMilliseconds / oldAverage.TotalMilliseconds;
        }
        public static double BenchmarkCompare(Action oldAction, Action newAction)
        {
            var oldTime = Benchmark(oldAction);
            var newTime = Benchmark(newAction);
            return newTime.TotalMilliseconds / oldTime.TotalMilliseconds;
        }



        /// <summary>
        /// Benchmark an action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>The time it takes to perform as TimeSpan.</returns>
        public static TimeSpan BenchmarkParallel(Action action, int count, int taskCount)
        {
            int started = 0;

            DateTime start = DateTime.Now;
            Task[] tasks = new Task[taskCount];
            for (int i = 0; i < taskCount; i++)
            {
                tasks[i] = Task.Run(ExecuteActions);
            }
            Task.WaitAll(tasks);
            var time = DateTime.Now - start;

            for (int i = 0; i < taskCount; i++)
            {
                tasks[i].Dispose();
            }
            return time;

            void ExecuteActions()
            {
                while (true)
                {
                    lock (tasks)
                    {
                        if (count <= started) break;
                        started++;
                    }
                    action.Invoke();
                }
            }
        }
        /// <summary>
        /// Benchmark an action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>The average time it takes to perform as TimeSpan.</returns>
        public static TimeSpan BenchmarkAverage(Action action, int count)
        {
            TimeSpan result = TimeSpan.Zero;
            for (int i = 0; i < count; i++)
            {
                result += Benchmark(action);
            }
            result /= count;
            return result;
        }
        /// <summary>
        /// Benchmark an action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>The times it takes to perform as TimeSpan[].</returns>
        public static TimeSpan[] Benchmark(Action action, int count)
        {
            TimeSpan[] result = new TimeSpan[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = Benchmark(action);
            }
            return result;
        }
        /// <summary>
        /// Benchmark an action.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>The time it takes to perform as TimeSpan.</returns>
        public static TimeSpan Benchmark(Action action)
        {
            DateTime start = DateTime.Now;
            action?.Invoke();
            return DateTime.Now - start;
        }
    }
}