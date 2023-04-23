using Pico.Arrays;
using Pico.Conversion;
using Pico.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Pico.Networking
{
    public class TcpNetCom
    {
        public static TcpClient WaitForConnection(int listenPort)
        {
            //client == null right now
            TcpListener listener = new TcpListener(IPAddress.Loopback, listenPort);
            listener.Start();
            var client = listener.AcceptTcpClient();
            listener.Stop();
            return client;
        }

        public static TcpClient ConnectToServer(IPAddress serverAddress, int serverPort)
        {
            //client == null right now
            var client = new TcpClient();
            client.Connect(serverAddress, serverPort);
            return client;
        }
    }
}
