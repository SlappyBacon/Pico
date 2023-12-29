using System.Net;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Pico.Randoms;

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
        public static IPAddress GetMyLocalIp()
        {
            var hostName = Dns.GetHostName();
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            if (addresses.Length < 1) return null;
            return addresses[addresses.Length - 1];
            
        }

        /// <summary>
        /// Returns this machine's public IP address
        /// </summary>
        /// <returns></returns>
        public static async Task<IPAddress> GetMyPublicIpAsync()
        {
            var httpClient = new HttpClient();
            var got = await httpClient.GetAsync("https://api.ipify.org");
            var ipAsString = await got.Content.ReadAsStringAsync();
            httpClient.Dispose();
            return IPAddress.Parse(ipAsString);
        }
        public static IPAddress GetMyPublicIp()
        {
            var task = GetMyPublicIpAsync();
            return task.Result;
        }
        
    }
}