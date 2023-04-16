using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pico.AsyncTools
{
    public static class AsyncTools
    {
        static TimeSpan _defaultTimeout = TimeSpan.FromSeconds(1);
        public static TimeSpan DefaultTimeout { get { return _defaultTimeout; } }

        public static async Task<bool> AddToQueueAsync(List<byte[]> queue, byte[] bytes, TimeSpan timeout)
        {
            if (timeout.TotalSeconds < 0) timeout = DefaultTimeout;
            DateTime endTime = DateTime.Now + timeout;
            while (true)
            {
                if (DateTime.Now > endTime) return false;

                bool didAdd = TryAddToQueue(ref queue, bytes);
                if (!didAdd)
                {
                    await Task.Delay(1);
                    continue;
                }
                Console.WriteLine("+");
                return true;
            }
        }
        public static bool TryAddToQueue(ref List<byte[]> queue, byte[] bytes, int maxQueueSize = -1)
        {
            lock (queue)
            {
                if (maxQueueSize > 0 && queue.Count >= maxQueueSize) return false;

                queue.Add(bytes);

                return true;
            }
        }

        public static async Task<byte[]> PopFromQueueAsync(List<byte[]> queue, TimeSpan timeout)
        {
            if (timeout.TotalSeconds < 0) timeout = DefaultTimeout;
            DateTime endTime = DateTime.Now + timeout;
            while (true)
            {
                if (DateTime.Now > endTime)
                {
                    return null;   //NULL
                }

                byte[] poppedFromQueue;
                bool didPop = TryPopFromQueue(ref queue, out poppedFromQueue);
                if (!didPop)
                {
                    //Check back later
                    await Task.Delay(1);
                    continue;
                }
                Console.WriteLine("-");
                return poppedFromQueue;
            }
        }
        public static bool TryPopFromQueue(ref List<byte[]> queue, out byte[] bytes)
        {
            lock (queue)
            {
                if (queue.Count < 1)
                {
                    bytes = null;  //NULL
                    return false;
                }

                bytes = queue[0];
                queue.RemoveAt(0);

                return true;
            }
        }
    }
}
