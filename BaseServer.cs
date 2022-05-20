using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pico.Networking
{
    public class BaseServer : IDisposable
    {
        //Servers must have their own director...

        Director serverDirector = null;
        bool serverRunning = false;
        Thread[] serveThreads = null;
        Action<NetCom> ServeAction = null;

        public BaseServer(Action<NetCom> SetServeAction, int incomingPort, int[] servePorts, string[] refs = null)
        {
            serverDirector = new Director(incomingPort, servePorts, refs);
            serveThreads = new Thread[serverDirector.ServePorts.Length];
            ServeAction = SetServeAction;
        }
        public BaseServer(Action<NetCom> SetServeAction, int incomingPort, int servePortMin, int servePortMax, string[] refs = null)
        {
            serverDirector = new Director(incomingPort, IntRange(servePortMin, servePortMax), refs);
            serveThreads = new Thread[serverDirector.ServePorts.Length];
            ServeAction = SetServeAction;
        }
        int[] IntRange(int min, int max)
        {
            int[] result = new int[max - min + 1];
            for (int i = 0; i < result.Length;)
            {
                result[i] = min + i;
                i++;
            }
            return result;
        }
        public void Start()
        {
            if (serverRunning) return;
            serverRunning = true;
            Console.WriteLine("STARTING SERVER");

            //Serve threads
            for (int i = 0; i < serveThreads.Length;)
            {
                serveThreads[i] = new Thread(new ParameterizedThreadStart(WaitForClient));
                serveThreads[i].Start(serverDirector.ServePorts[i]);
                i++;
            }
            serverDirector.Start();
        }
        public void Stop()
        {
            if (!serverRunning) return;
            serverRunning = false;
            Console.WriteLine("STOPPING SERVER");

            //Join all threads
            //'Poke' each TcpCom to wake them up.
            serverDirector.Stop();
            for (int i = 0; i < serveThreads.Length;)
            {
                new NetCom(IpTools.MyLocalIp(), serverDirector.ServePorts[i]).Dispose();
                serveThreads[i].Join();
                i++;
            }
        }


        void WaitForClient(object portObj)
        {
            //[Thread Entry]
            int port = (int)portObj;
            if (port < 0) return;
            if (ServeAction == null) return;
            while (true)
            {
                if (!serverRunning) break;
                ServeNextClient(port);
            }
        }

        //Waits for clients to connect, then serve
        void ServeNextClient(int port)
        {
            using (NetCom com = new NetCom(port))
            {
                if (!serverRunning) return;

                //Verify
                if (com.ReadText() != "where") return;
                if (!com.WriteText("here")) return;

                //START OF TRANSACTION
                serverDirector.servePortBusy[port] = true;
                ServeAction.Invoke(com);
                serverDirector.servePortBusy[port] = false;
                //END OF TRANSACTION
            }
        }


        public void Dispose()
        {
            serverDirector.Dispose();
            Stop();
        }


    }
}
