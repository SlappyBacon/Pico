using System;

namespace Pico.Networking
{
    public class BaseClient : IDisposable
    {
        /// <summary>
        /// VERSION 4
        /// 03/09/12
        /// </summary>
        /// 
        
        NetCom com;
        public bool WriteText(string text) => com.WriteText(text);
        public string ReadText() => com.ReadText();





        public bool IsConnected { get { return com != null && com.IsConnected; } }
        /// <summary>
        /// Communicates with Director and BaseServer objects.
        /// </summary>
        /// <param name="ip">ComServer IP</param>
        /// <param name="port">ComServer Port</param>
        public BaseClient(string serverIp, int mainPort)
        {
            TryToConnect(serverIp, mainPort);
        }

        bool TryToConnect(string ip, int port)
        {
            if (IsConnected) return false;   //Check if already connected
            Console.WriteLine("connecting...");
            
            FindConnection(ip, port);
            if (!IsConnected)
            {
                Console.WriteLine("failed to connect.");
                return false;
            }
            Console.WriteLine("connected.");
            return true;
        }

        //Communicates with the com that tells you which port to be served on
        public bool FindConnection(string ip, int port)
        {
            Console.WriteLine($"=> {ip}:{port}");
            //Ask where
            if (IsConnected) com.Dispose();
            com = new NetCom(ip, port);
            var askedWhere = com.WriteText("where");
            if (!askedWhere) return false;
            //Expect either a "here" response, or an ip:port response
            var where = com.ReadText();
            if (where == null) return false;
            if (where == "here") return true;   //Start getting served on this ip:port
            int col = where.IndexOf(':');
            if (col != -1)
            {
                string ipMaybe = where.Substring(0, col);
                if (ipMaybe == "here") ipMaybe = com.IpAddress;
                string portMaybe = where.Substring(++col);
                return FindConnection(ipMaybe, int.Parse(portMaybe));
            }
            return false;
        }
        
        public void Dispose()
        {
            if (com != null) com.Dispose();
        }
    }
}
