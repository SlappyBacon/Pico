using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Pico.Networking;

class UdpNetCom : IDisposable
{
    // Create a new UdpClient and bind it to a local endpoint
    UdpClient udpClient;
    public Action<IPEndPoint, byte[]> OnBytesReceived = null;
    public Action<IPEndPoint, byte[]> OnBytesSent = null;

    public UdpNetCom()
    {
        udpClient = new UdpClient();
    }

    public UdpNetCom(int myLocalPort) //Where I receive messages
    {
        udpClient = new UdpClient(myLocalPort);
    }
    


    public bool WriteBytes(byte[] bytes, string toAddress, int toPort)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(toAddress), toPort);
        return WriteBytes(bytes, endpoint);
    }
    public bool WriteBytes(byte[] bytes, IPAddress toAddress, int toPort)
    {
        var endpoint = new IPEndPoint(toAddress, toPort);
        return WriteBytes(bytes, endpoint);
    }
    public bool WriteBytes(byte[] bytes, IPEndPoint endpoint)
    {
        if (udpClient == null) return false;
        if (bytes == null) return false;
        if (endpoint == null) return false;

        int sent = udpClient.Send(bytes, bytes.Length, endpoint);
        OnBytesSent?.Invoke(endpoint, bytes);
        return sent == bytes.Length;
    }

    public byte[] ReadBytesFromAny()
    {
        var endpoint = new IPEndPoint(IPAddress.Any, 0);
        return ReadBytes(ref endpoint);
    }
    public byte[] ReadBytesFromAny(out IPEndPoint sender)
    {
        var endpoint = new IPEndPoint(IPAddress.Any, 0);
        var readBytes = ReadBytes(ref endpoint);
        sender = endpoint;
        return readBytes;
    }
    public byte[] ReadBytes(string fromAddress, int port)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(fromAddress), port);
        return ReadBytes(ref endpoint);
    }
    public byte[] ReadBytes(IPAddress fromAddress, int port)
    {
        var endpoint = new IPEndPoint(fromAddress, port);
        return ReadBytes(ref endpoint);
    }
    public byte[] ReadBytes(ref IPEndPoint fromEndPoint)
    {
        if (udpClient == null) return null;
        if (fromEndPoint == null) return null;
        var read = udpClient.Receive(ref fromEndPoint);
        OnBytesReceived?.Invoke(fromEndPoint, read);
        return read;
    }

    public void Dispose()
    {
        OnBytesSent = null;
        OnBytesReceived = null;
        udpClient.Dispose();
    }
}
