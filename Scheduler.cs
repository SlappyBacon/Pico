using Pico.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pico.Jobs
{
    public class Scheduler : IDisposable
    {
        public int ThreadCount { get { return workerThreads.Length; } }
        public int JobsScheduled { get { return schedule.Count; } }
        public int JobsActive
        {
            get
            {
                int count = 0;
                for (int i = 0; i < workerThreads.Length; i++)
                {
                    if (threadJobId.ElementAt(0).Value != -1)
                    {
                        count++;
                    }
                }
                return count;
            }
        }


        private Dictionary<int, Delegate> schedule;
        private Dictionary<int, object[]> scheduleArgs;
        private Dictionary<int, int> threadJobId;
        private Dictionary<int, object> results;
        private bool disposing = false;
        private int nextScheduleId = 0;
        private Thread[] workerThreads;



        /// <summary>
        /// Creates a job scheduler.
        /// </summary>
        /// <param name="threadCount">Number of dedicated threads. (Max amount of simultanious jobs)</param>
        public Scheduler(int threadCount = 4)
        {
            if (threadCount > Environment.ProcessorCount) threadCount = Environment.ProcessorCount;
            schedule = new Dictionary<int, Delegate>(threadCount);
            scheduleArgs = new Dictionary<int, object[]>(threadCount);
            threadJobId = new Dictionary<int, int>(threadCount);
            results = new Dictionary<int, object>(threadCount);
            workerThreads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threadJobId[i] = -1;
                workerThreads[i] = new Thread(new ParameterizedThreadStart(WorkerThreadAction));
                workerThreads[i].Start(i);
            }
        }

        //Each worker thread checks the schedule to see if there's any jobs to be done.
        //If there's a job, claim it and get started.
        //If not, chill out and check again.
        private void WorkerThreadAction(object _id)
        {
            int threadId = (int)_id;


            int count = 0;
            bool wait = false;
            while (true)
            {
                if (disposing) break;

                if (wait) Thread.Sleep(1); //thanks - from cpu
                wait = false;

                int scheduleId;
                Delegate del;
                object[] delArgs = null;
                lock (schedule)
                {
                    lock (scheduleArgs)
                    {
                        if (schedule.Count < 1)
                        {
                            wait = true;
                            continue;
                        }
                        scheduleId = schedule.ElementAt(0).Key;
                        del = schedule[scheduleId];
                        schedule.Remove(scheduleId);
                        if (scheduleArgs.ContainsKey(scheduleId))
                        {
                            delArgs = scheduleArgs[scheduleId];
                            scheduleArgs.Remove(scheduleId);
                        }
                    }
                }

                //Invoke and store result
                lock (threadJobId)
                {
                    //Is now busy
                    threadJobId[threadId] = scheduleId;
                }
                try
                {
                    var result = del.DynamicInvoke(delArgs);
                    if (result != null)
                    {
                        lock (results)
                        {
                            results[scheduleId] = result;
                        }
                    }
                    count++;
                    //Console.WriteLine($"[{threadId}-{scheduleId}] Complete");
                }
                catch
                {
                    //Console.WriteLine($"[{threadId}-{scheduleId}] Error");
                }
                lock (threadJobId)
                {
                    //Not busy anymore
                    threadJobId[threadId] = -1;
                }

            }
            //Console.WriteLine($"[{threadId}] Exited ({count} Jobs Complete)");
        }





        #region Schedule
        public int Schedule(Delegate del, object arg1) => Schedule(del, new object[] { arg1 });
        public int Schedule(Delegate del, object arg1, object arg2) => Schedule(del, new object[] { arg1, arg2 });
        public int Schedule(Delegate del, object arg1, object arg2, object arg3) => Schedule(del, new object[] { arg1, arg2, arg3 });
        public int Schedule(Delegate del, object arg1, object arg2, object arg3, object arg4) => Schedule(del, new object[] { arg1, arg2, arg3, arg4 });
        public int Schedule(Delegate del, object[] args = null)
        {
            //Schedule then return the delegate id
            if (del == null) return -1;
            lock (schedule)
            {
                lock (scheduleArgs)
                {
                    schedule[nextScheduleId] = del;
                    if (args != null)
                    {
                        scheduleArgs[nextScheduleId] = args;
                    }
                }
            }
            return nextScheduleId++;
        }
        #endregion
        #region GetResult
        /// <summary>
        /// Waits for job to be finished, then returns the result.
        /// If the job doesn't exist, then null will be returned.
        /// </summary>
        /// <param name="jobId">Job Id</param>
        /// <returns></returns>
        public object GetResult(int jobId)
        {
            WaitForJobDone(jobId);

            object result = null;
            lock (results)
            {
                if (results.ContainsKey(jobId))
                {
                    result = results[jobId];
                    results.Remove(jobId);
                }
            }
            return result;
        }
        #endregion
        //Returns the thread a job is currently running on, or -1
        private int ThreadRunningJob(int jobId)
        {
            for (int i = 0; i < threadJobId.Count; i++)
            {
                if (threadJobId.ElementAt(i).Value == jobId)
                {
                    return i;
                }
            }
            return -1;
        }
        #region Wait For Things
        /// <summary>
        /// If the job is scheduled, or currently active, then wait.
        /// </summary>
        /// <param name="jobId">Job Id</param>
        public void WaitForJobDone(int jobId)
        {
            //Check not waiting to begin
            while (schedule.ContainsKey(jobId)) Thread.Sleep(1);
            //Check still running
            while (ThreadRunningJob(jobId) != -1) Thread.Sleep(1);
        }
        
        /// <summary>
        /// If any tasks are scheduled, or currently active, then wait.
        /// </summary>
        public void WaitForAllDone()
        {
            while (true)
            {
                Thread.Sleep(1);
                if (JobsActive > 0) continue;
                //All threads not busy
                if (schedule.Count > 0) continue;
                //No more scheduled tasks
                break;
            }
        }
        #endregion
        #region Dispose
        /// <summary>
        /// Waits for current tasks to be completed.
        /// Closes threads, then clears data.
        /// 
        /// DOES NOT complete remaining scheduled tasks.
        /// Call WaitForAllDone() if you want all tasks to be completed before disposal.
        /// </summary>
        public void Dispose()
        {
            //Wait for current tasks to be completed.
            disposing = true;
            ThreadTools.JoinThreads(workerThreads);

            lock (schedule)
            {
                lock (scheduleArgs)
                {
                    lock (results)
                    {
                        schedule.Clear();
                        scheduleArgs.Clear();
                        results.Clear();
                    }
                }
            }
            //Console.WriteLine("Scheduler disposal complete.");
        }
        #endregion
    }
}
