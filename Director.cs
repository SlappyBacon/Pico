using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pico.Networking
{
    public class Director : IDisposable
    {




        //Revisit this...  It's kinda doo-doo.

        //Replace all this with a BaseServer, then set the action to assigning things...








        public int[] ServePorts { get { return servePorts; } }

        int assignPort;
        int[] servePorts;
        string[] mirrorServerAddresses;


        bool serverRunning = false;
        public Dictionary<int, bool> servePortBusy;
        Thread assignThread;

        
        
        public Director(int setAssignPort, int[] setServePorts = null, string[] setMirrorServerAddresses = null)
        {
            assignPort = setAssignPort;
            servePorts = setServePorts;
            if (servePorts == null) servePorts = new int[0];
            mirrorServerAddresses = setMirrorServerAddresses;  
            if (mirrorServerAddresses == null) mirrorServerAddresses = new string[0];
            servePortBusy = new Dictionary<int, bool>(servePorts.Length);
            for (int i = 0; i < servePorts.Length; i++) servePortBusy[servePorts[i]] = false;//All available
        }
        public Director(int setAssignPort, int setMinServePort, int setMaxServePort, string[] setMirrorServerAddresses = null)
        {
            assignPort = setAssignPort;
            servePorts = IntRange(setMinServePort, setMaxServePort);
            mirrorServerAddresses = setMirrorServerAddresses;
            if (mirrorServerAddresses == null) mirrorServerAddresses = new string[0];
            servePortBusy = new Dictionary<int, bool>(servePorts.Length);
            for (int i = 0; i < servePorts.Length; i++) servePortBusy[servePorts[i]] = false;//All available
        }

        public void Start()
        {
            if (serverRunning) return;
            serverRunning = true;
            Console.WriteLine("STARTING DIRECTOR");

            //Port-Assign thread
            assignThread = new Thread(new ParameterizedThreadStart(AssignClientsToPortsAction));
            assignThread.Start(assignPort);
        }

        public void Stop()
        {
            if (!serverRunning) return;
            serverRunning = false;
            Console.WriteLine("STOPPING DIRECTOR");
            //'Poke' TcpCom to wake them up.
            new NetCom(IpTools.MyLocalIp(), assignPort).Dispose();
            assignThread.Join();
        }

        //This thread is specifically for assigning connections.
        //If all ports on this location are taken up, then your mirror server's 'assigner thread' will be suggested.
        void AssignClientsToPortsAction(object portObj)
        {
            //[Thread Entry]
            int port = (int)portObj;
            if (port < 0) return;
            //Gets a request for a port, reply with the next available thread/port
            while (true)
            {
                if (!serverRunning) break;
                AssignNextClientToPort(port);
            }
        }
        void AssignNextClientToPort(int assignPort)
        {
            using (NetCom com = new NetCom(assignPort))
            {
                if (!serverRunning) return;

                //get where request
                var where = com.ReadText();
                if (where == null) return;
                if (where != "where") return;

                //Reply with next avalable unused serve port
                bool resultFound = false;
                string whereReply = "unknown";
                for (int i = 0; i < servePorts.Length;)
                {
                    if (!servePortBusy[servePorts[i]])
                    {
                        whereReply = $"here:{servePorts[i]}";    //Suggest connecting here
                        resultFound = true;
                        break;
                    }
                    i++;
                }

                //Suggest mirror server, if specified ?
                if (!resultFound)
                {
                    if (mirrorServerAddresses.Length > 0)
                    {
                        int addr = Random.Shared.Next(0,mirrorServerAddresses.Length);
                        whereReply = $"{mirrorServerAddresses[addr]}";
                    }
                }

                //reply
                var replySent = com.WriteText(whereReply);
                if (!replySent) return;
            }
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
        public void Dispose()
        {
            Stop();
        }
    }
}
