using System;
using System.Collections.Generic;
using System.Threading;

namespace Pico.Jobs
{
    //The intention of this project was to make
    //multithreading tasks easier.

    public class JobHandler
    {
        

        static Thread thread = new Thread(Handler);
        static int maxWorkerThreads;
        static List<Job> jobs = new List<Job>(10);


        public JobHandler(int maxThreads = 4)
        {
            maxWorkerThreads = maxThreads;
            //constrain thread count to env.processorcount-1



            thread.Start();
        }

        

        public void Schedule(Job job)
        {
            
        }



        static void Handler()
        {
            while (true)
            {
                Console.WriteLine("tick");


                Thread.Sleep(250);



            }
        }


    }


    

}