using System.Net;
using System.Net.NetworkInformation;
namespace Pico.Networking;

/// <summary>
/// A collection of port-related tools.
/// </summary>
public static class PortTools
{
    /// <summary>
    /// Returns the next free port.
    /// </summary>
    /// <param name="port">Port to begin searching at.</param>
    /// <returns></returns>
    public static int NextFreePort(int port = 0)
    {
        var properties = IPGlobalProperties.GetIPGlobalProperties();
        if (port < 0) port = 0;
        while (!IsFree(port, properties)) port += 1;
        return port;
    }

    /// <summary>
    /// Returns if the port is free.
    /// </summary>
    /// <param name="port">Port.</param>
    /// <param name="properties">Properties.</param>
    /// <returns></returns>
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
