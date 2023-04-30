using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pico.AsyncTools
{
    public static class AsyncTools
    {
        public static Task[] ForEachAsync<T>(T[] array, Action<T> action)
        {
            Task[] tasks = new Task[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                T item = array[i];
                tasks[i] = Task.Run(() => action(item));
            }

            return tasks;
        }

    }
}
