using System;
using System.Threading;

namespace Pico.Threads
{
    /// <summary>
    /// A collection of tools for working with threads.
    /// </summary>
    public static class ThreadTools
    {
        /// <summary>
        /// Looks for an available thread in an array.
        /// </summary>
        /// <param name="threads">Threads to look through.</param>
        /// <param name="index">Index of found thread.  -1 if none available.</param>
        /// <returns></returns>
        public static bool FindFreeThread(in Thread[] threads, out int index)
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