using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pico.Async
{
    public static class AsyncTools
    {
        /// <summary>
        /// Takes an array of objects 
        /// and performs an action to each.
        /// A new task is created for each action
        /// and is returned as an array.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="array">Array of objects for the action to be performed on.</param>
        /// <param name="action">Action to be performed on each object.</param>
        /// <returns></returns>
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
