using Pico.Threads;
using System;
using System.Threading;

namespace Pico.Jobs
{
    //The intention of this project was to make
    //multithreading tasks easier.
    public class Job : IDisposable
    {

        

        private static Thread[]? workThreads;

        private static Action<object>[]? actions;    // Each workload consists of an action
        private static object[]? args;               // And optional arguments, which you will have to parse from within the action.

        private static bool started = false;

        public Job(Action<object> action, object? actionArgs = null)
        {
            actions = new Action<object>[] { action };
            args = new object[] { actionArgs };
        }
        public Job(Action<object> action, object[] allActionArgs)
        {
            actions = new Action<object>[] { action };
            args = allActionArgs;
        }
        public Job(Action<object>[] allActions, object? actionArgs = null)
        {
            actions = allActions;
            args = new object[] { actionArgs };
        }
        public Job(Action<object>[] allActions, object[] allActionArgs)
        {
            actions = allActions;
            args = allActionArgs;
        }


        public void Execute(int threadCount = 1)
        {
            //Only able to run once
            if (started) return;
            started = true;

            //Are there any actions?
            if (actions == null) return;
            if (actions.Length == 0) return;

            //Constrain thread count to environment
            //if (threadCount > Environment.ProcessorCount) threadCount = Environment.ProcessorCount;
            if (threadCount < 1) threadCount = 1;

            if (actions.Length == 1) SingleActionMode(threadCount);
            else MultiActionMode(threadCount);
            //all work has been assigned
        }


        public void WaitForEnd()
        {
            ThreadTools.JoinThreads(workThreads);
        }




        static void MultiActionMode(int threadCount)
        {
            if (threadCount > actions.Length) threadCount = actions.Length;
            workThreads = new Thread[threadCount];
            if (threadCount == 1) MultiActionSingleThread();
            else MultiActionMultiThread(threadCount);
            //All assigned
        }
        static void MultiActionSingleThread()
        {
            workThreads[0] = new Thread(() => MultiActionSingleThreadAction());
            workThreads[0].Start();
        }
        static void MultiActionSingleThreadAction()
        {
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] == null) continue;
                //Start with/without args
                if (args == null || args.Length <= i) actions[i](null);
                else actions[i](args[i]);
                //next
            }
        }
        static void MultiActionMultiThread(int threadCount)
        {
            workThreads = new Thread[threadCount];
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] == null) continue;

                int freeThreadIndex;
                while (!ThreadTools.FindFreeThread(workThreads, out freeThreadIndex)) Thread.Sleep(25);
                workThreads[freeThreadIndex] = new Thread(new ParameterizedThreadStart(actions[i]));
                //Start with/without args
                if (args == null || args.Length < i || args[i] == null) workThreads[freeThreadIndex].Start(null);
                else workThreads[freeThreadIndex].Start(args[i]);
                //next
            }
        }
        









        static void SingleActionMode(int threadCount)
        {
            //Execute that one action multiple times, but pass it different args

            if (args == null || args.Length == 0) args = new object[] { null };

            // [1], [1,2,3,...]
            if (threadCount > args.Length) threadCount = args.Length;
            
            workThreads = new Thread[threadCount];
            
            if (threadCount == 1) SingleActionSingleThread();
            else SingleActionMultiThread(threadCount);
            //All assigned
        }


        static void SingleActionSingleThread()
        {
            workThreads[0] = new Thread(() => SingleActionSingleThreadAction());
            workThreads[0].Start();
        }

        static void SingleActionSingleThreadAction()
        {
            for (int i = 0; i < args.Length; i++)
            {
                //Start with/without args
                if (args == null) actions[0](null);
                else actions[0](args[i]);
                //next
            }
        }

        static void SingleActionMultiThread(int threadCount)
        {
            workThreads = new Thread[threadCount];
            Console.WriteLine(workThreads.Length);
            for (int i = 0; i < args.Length; i++)
            {
                int freeThreadIndex;
                while (!ThreadTools.FindFreeThread(workThreads, out freeThreadIndex)) Thread.Sleep(25);
                workThreads[freeThreadIndex] = new Thread(new ParameterizedThreadStart(actions[0]));
                //Start with/without args
                if (args == null) workThreads[freeThreadIndex].Start(null);
                else workThreads[freeThreadIndex].Start(args[i]);
                //next
            }
        }



        public void Dispose()
        {
            
        }
    }
}