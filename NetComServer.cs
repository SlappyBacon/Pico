using Pico.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pico.Networking
{
    internal class NetComServer : IDisposable
    {
        int port;
        bool running = false;
        Thread runThread = null;
        Action<NetCom> serveAction;



        public NetComServer(int port, Action<NetCom> serveAction)
        {
            this.port = port;
            this.serveAction = serveAction;
        }



        public void Start()
        {
            if (running) return;
            running = true;
            runThread = new Thread(RunThreadAction);
            runThread.Start();
        }

        void RunThreadAction()
        {
            while (true)
            {
                if (!running) break;
                //Serve next request
                using (NetCom com = new NetCom(port))
                {
                    serveAction(com);
                }
            }
        }

        public void Stop()
        {
            if (!running) return;
            running = false;
            new NetCom(IpTools.MyLocalIp(), port);
            ThreadTools.JoinThread(runThread);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
