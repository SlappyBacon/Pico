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
        /// Define the function to be performed, and its input arg(S).
        /// </summary>
        public ParaVariable(Delegate del, object arg1) => Setup(del, new object[] { arg1 });
        public ParaVariable(Delegate del, object arg1, object arg2) => Setup(del, new object[] { arg1, arg2 });
        public ParaVariable(Delegate del, object arg1, object arg2, object arg3) => Setup(del, new object[] { arg1, arg2, arg3 });
        public ParaVariable(Delegate del, object arg1, object arg2, object arg3, object arg4) => Setup(del, new object[] { arg1, arg2, arg3, arg4 });
        public ParaVariable(Delegate del, object arg1, object arg2, object arg3, object arg4, object arg5) => Setup(del, new object[] { arg1, arg2, arg3, arg4, arg5 });
        public ParaVariable(Delegate del, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6) => Setup(del, new object[] { arg1, arg2, arg3, arg4, arg5, arg6 });
        public ParaVariable(Delegate del, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7) => Setup(del, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7 });
        public ParaVariable(Delegate del, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8) => Setup(del, new object[] { arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 });
        public ParaVariable(Delegate del, object[] args = null) => Setup(del, args);

        void Setup(Delegate del, object[] args)
        {
            thread = new Thread(new ParameterizedThreadStart(ThreadAction));
            ThreadData data = new ThreadData(del, args);
            thread.Start(data);
        }

        void ThreadAction(object _data)
        {
            ThreadData data = (ThreadData)_data;
            try
            {
                value = data.Del.DynamicInvoke(data.Args);
            }
            catch
            {
                value = null;   //Args Mismatch
            }
        }
        
        public void Dispose()
        {
            if (!joined) ThreadTools.JoinThread(thread);
        }

        struct ThreadData
        {
            public Delegate Del;
            public object[] Args;
            public ThreadData(Delegate del, object[] args)
            {
                Del = del;
                Args = args;
            }
        }

        public override string ToString() => Value?.ToString();
    }
}
