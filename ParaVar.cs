using System;
using System.Threading;

namespace Pico.Threads
{
    /// <summary>
    /// Runs a function on a seperate thread, which
    /// updates the value apon completion.
    /// </summary>
    class ParaVariable
    {
        object value = null;
        Thread thread = null;

        //Add support for input parameters



        public ParaVariable(object function)
        {
            thread = new Thread(new ParameterizedThreadStart(ThreadAction));
            thread.Start(function);
        }

        void ThreadAction(object args)
        {
            Func<object> function = args as Func<object>;
            value = function();
        }
        /// <summary>
        /// Waits for function to complete,
        /// then returns value as an object.
        /// </summary>
        /// <returns></returns>
        public object Get()
        {
            ThreadTools.JoinThread(thread);
            return value;
        }
    }
}
