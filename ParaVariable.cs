using System;
using System.Threading;

namespace Pico.Threads
{
    /// <summary>
    /// Runs a function on a seperate thread, which
    /// updates the value apon completion.
    /// </summary>
    class ParaVariable : IDisposable
    {
        object value = null;
        Thread thread = null;
        bool joined = false;
        /// <summary>
        /// Waits for function to complete,
        /// then returns value as an object.
        /// </summary>
        /// <returns></returns>
        public object Value 
        { 
            get 
            {
                if (!joined) ThreadTools.JoinThread(thread);
                joined = true;
                return value;
            }
        }
        public bool IsFinished
        {
            get { return thread == null || !thread.IsAlive; }
        }
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
        
        public void Dispose()
        {
            if (!joined) ThreadTools.JoinThread(thread);
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
