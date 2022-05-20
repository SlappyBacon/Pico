using System;
using System.Threading;

namespace Pico.Threads
{
    public static class ThreadTools
    {
        
        public static void JoinThread(Thread thread)
        {
            if (thread == null) return;
            thread.Join();
        }

        public static void JoinThreads(Thread[] threads)
        {
            if (threads == null) return;
            foreach (Thread thread in threads) JoinThread(thread);
        }

        public static bool FindFreeThread(Thread[] threads, out int index)
        {
            if (threads == null)
            {
                index = -1;
                return false;
            }

            for (int i = 0; i < threads.Length; i++)
            {
                if (threads[i] == null || threads[i].ThreadState == ThreadState.Stopped)
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }
    }
}