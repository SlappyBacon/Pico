using Pico.Jobs;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Pico.Networking
{
	public static class IpTools
    {

        public static string[] GetLocalDeviceIps()
        {
            string[] possibleIps = new string[256];

            for (int i = 0; i < possibleIps.Length;)
            {
                possibleIps[i] = $"192.168.0.{i}";
                i++;
            }

            List<string> foundIps = new List<string>();

            Job job = new Job(TryIp, possibleIps);
            job.Execute(256);
            job.WaitForEnd();

            return foundIps.ToArray();


            void TryIp(object args)
            {
                string ip = (string)args;
                var pingInfo = new Ping().Send(ip);
                if (pingInfo.Status == IPStatus.Success) return;    //ADD TO FOUND IPS :) OKOK BYE
            }

        }

        /// <summary>
        /// Returns this machine's Local IP Address
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
        /// Returns this machine's Public IP Address
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
        /// Returns a TcpClient's Public IP Address
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