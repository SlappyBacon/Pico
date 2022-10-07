using System.Net;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Pico.Networking
{
    /// <summary>
    /// A collection of IP tools.
    /// </summary>
	public static class IpTools
    {
        /// <summary>
        /// Returns this machine's local IP address
        /// </summary>
        /// <returns></returns>
        public static string MyLocalIp()
        {
            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
                if (addresses.Length > 0) return addresses[addresses.Length - 1].ToString();
            }
            catch
            {
                
            }
            return null;
        }

        /// <summary>
        /// Returns this machine's public IP address
        /// </summary>
        /// <returns></returns>

        public static string MyPublicIp()
        {
            try
            {
                return new WebClient().DownloadString("https://api.ipify.org");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a TcpClient's public IP address
        /// </summary>
        /// <returns></returns>
        public static string ClientIp(TcpClient client)
        {
            if (client == null) return null;
            if (client.Client == null) return null;
            if (client.Client.RemoteEndPoint == null) return null;
            var ip = (IPEndPoint)client.Client.RemoteEndPoint;
            return ip.Address.ToString();
        }
    }
}