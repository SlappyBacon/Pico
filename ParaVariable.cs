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

        /// <summary>
        /// Define the function to be performed, and it's input arg.
        /// </summary>
        /// <param name="function">Must take in an object, and return an object.</param>
        /// <param name="args">Object to pass into the function.</param>
        public ParaVariable(object function, object args)
        {
            thread = new Thread(new ParameterizedThreadStart(ThreadAction));
            ThreadData threadData = new ThreadData((Func<object,object>)function, args);
            thread.Start(threadData);
        }

        void ThreadAction(object args)
        {
            ThreadData data = (ThreadData)args;
            value = data.Function(data.Args);
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
        struct ThreadData
        {
            public Func<object, object> Function;
            public object Args;
            public ThreadData(Func<object,object> setFunction, object args)
            {
                Function = setFunction;
                Args = args;
            }
        }
    }
}
