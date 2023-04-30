using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Pico.Networking;

public static class PortTools
{
    public static int NextFreePort(int port = 0)
    {
        var properties = IPGlobalProperties.GetIPGlobalProperties();
        if (port < 0) port = 0;
        while (!IsFree(port, properties)) port += 1;
        return port;
    }
    public static bool IsFree(int port, IPGlobalProperties properties = null)
    {
        if (properties == null)
        {
            properties = IPGlobalProperties.GetIPGlobalProperties();
        }
        return IsTcpFree(port, properties) && IsUdpFree(port, properties);
    }
    static bool IsTcpFree(int port, IPGlobalProperties properties = null)
    {
        if (properties == null)
        {
            properties = IPGlobalProperties.GetIPGlobalProperties();
        }
        IPEndPoint[] listeners = properties.GetActiveTcpListeners();
        int[] openPorts = listeners.Select(item => item.Port).ToArray<int>();
        return openPorts.All(openPort => openPort != port);
    }
    static bool IsUdpFree(int port, IPGlobalProperties properties = null)
    {
        if (properties == null)
        {
            properties = IPGlobalProperties.GetIPGlobalProperties();
        }
        IPEndPoint[] listeners = properties.GetActiveUdpListeners();
        int[] openPorts = listeners.Select(item => item.Port).ToArray<int>();
        return openPorts.All(openPort => openPort != port);
    }
}
