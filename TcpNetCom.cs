using System.Net;
using System.Net.Sockets;

namespace Pico.Networking
{
    /// <summary>
    /// Tools for making TCP connections.
    /// </summary>
    public class TcpNetCom
    {
        /// <summary>
        /// Wait for connection from client.
        /// </summary>
        /// <param name="listenPort">Port to listen at.</param>
        /// <returns></returns>
        public static TcpClient WaitForConnection(int listenPort)
        {
            //client == null right now
            TcpListener listener = new TcpListener(IPAddress.Loopback, listenPort);
            listener.Start();
            var client = listener.AcceptTcpClient();
            listener.Stop();
            listener.Dispose();
            return client;
        }

        /// <summary>
        /// Try to connect to server.
        /// </summary>
        /// <param name="serverAddress">Server's address.</param>
        /// <param name="serverPort">Server's port.</param>
        /// <returns></returns>
        public static TcpClient ConnectToServer(IPAddress serverAddress, int serverPort)
        {
            //client == null right now
            var client = new TcpClient();
            client.Connect(serverAddress, serverPort);
            return client;
        }
    }
}
