using System.Net;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System;
using System.Threading.Tasks;
using System.Net.Http;

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
            var hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            if (addresses.Length > 0)
            {
                return addresses[addresses.Length - 1].ToString();
            }
            return null;
        }

        /// <summary>
        /// Returns this machine's public IP address
        /// </summary>
        /// <returns></returns>

        public static async Task<string> MyPublicIp(HttpClient httpClient = null)
        {
            if (httpClient == null) httpClient = new HttpClient();
            var got = await httpClient.GetAsync("https://api.ipify.org");
            return await got.Content.ReadAsStringAsync();
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

        public static bool IsValidIpAddress(string ipAddress)
        {
            //0.0.0.0         (7 chars)
            //999.999.999.999 (15 chars)
            if (ipAddress.Length < 7) return false;
            if (ipAddress.Length > 15) return false;

            //Must be exactly 3 dots.
            int dots = 0;

            //All characters must be either a number, or '.'
            for (int i = 0; i < ipAddress.Length; i++)
            {
                bool isDot = ipAddress[i] == '.';
                if (
                    ipAddress[i] != '0' &&
                    ipAddress[i] != '1' &&
                    ipAddress[i] != '2' &&
                    ipAddress[i] != '3' &&
                    ipAddress[i] != '4' &&
                    ipAddress[i] != '5' &&
                    ipAddress[i] != '6' &&
                    ipAddress[i] != '7' &&
                    ipAddress[i] != '8' &&
                    ipAddress[i] != '9' &&
                    !isDot
                    ) return false;         //Invalid Character
                if (isDot) dots++;          //Increment Dots Count
            }

            if (dots != 3) return false;    //Wrong amount of dots.

            return true;    //Is Valid :)
        }

    }
}