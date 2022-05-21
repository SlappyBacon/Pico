using System;
using System.Collections.Generic;
using System.Threading;

namespace Pico.Jobs
{
    //The intention of this project was to make
    //multithreading tasks easier.

    /// <summary>
    /// ---WORK IN PROGRESS---
    /// Allows you to schedule Jobs, and run
    /// them automatically when resources are available.
    /// </summary>
    public class JobScheduler
    {
        

        static Thread thread = new Thread(Handler);
        static int maxWorkerThreads;
        static List<Job> jobs = new List<Job>(10);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxThreads">Max number of simultanious job threads.</param>
        public JobScheduler(int maxThreads = 4)
        {
            maxWorkerThreads = maxThreads;
            //constrain thread count to env.processorcount-1
            thread.Start();
        }

        
        /// <summary>
        /// NOT YET IMPLEMENTED
        /// </summary>
        /// <param name="job"></param>
        public void Schedule(Job job)
        {
            //IMPLEMENT
        }


        /// <summary>
        /// NOT YET IMPLEMENTED
        /// </summary>
        static void Handler()
        {
            //IMPLEMENT
        }


    }


    

}