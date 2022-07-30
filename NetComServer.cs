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
        object serveAction = null;  //Action<NetCom or EncryptedNetCom>




        public NetComServer(int port, Action<NetCom> serveAction)
        {
            this.port = port;
            this.serveAction = serveAction;
        }
        public NetComServer(int port, Action<EncryptedNetCom> serveAction)
        {
            this.port=port;
            this.serveAction = serveAction;
        }


        public void Start()
        {
            //Begin Start
            if (running) return;
            running = true;
            //Create Thread
            runThread = new Thread(RunThreadAction);
            runThread.Start();
        }

        void RunThreadAction()
        {
            while (true)
            {
                if (!running) break;
                //Serve next request

                //Raw
                if (serveAction is Action<NetCom>)
                {
                    var action = serveAction as Action<NetCom>;
                    using (NetCom com = new NetCom(port))
                    {
                        action(com);
                    }
                }
                //Encrypted
                else if (serveAction is Action<EncryptedNetCom>)
                {
                    var action = serveAction as Action<EncryptedNetCom>;
                    using (EncryptedNetCom com = new EncryptedNetCom(port))
                    {
                        action(com);
                    }
                }
            }
        }

        public void Stop()
        {
            //Begin stop
            if (!running) return;
            running = false;
            //Poke thread
            new NetCom(IpTools.MyLocalIp(), port).Dispose();
            //Join
            ThreadTools.JoinThread(runThread);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
