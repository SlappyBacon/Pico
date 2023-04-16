using Pico.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pico.Networking;

class UdpNetCom : IDisposable
{
    // Create a new UdpClient and bind it to a local endpoint
    UdpClient _udpClient;
    UdpClient UdpClient { get { return _udpClient; } }
    
    public int Port 
    { 
        get 
        {
            if (_udpClient == null) return -1;
            if (_udpClient.Client == null) return -1;
            if (_udpClient.Client.LocalEndPoint == null) return -1;
            var ipEp = (IPEndPoint)_udpClient.Client.LocalEndPoint;
            return ipEp.Port;
        } 
    }

    public const int DefaultTimeoutMilliseconds = 2000;

    public UdpNetCom()
    {
        _udpClient = new UdpClient();
    }

    public UdpNetCom(int myLocalPort) //Where I receive messages
    {
        _udpClient = new UdpClient(myLocalPort);
        _udpClient.Client.ReceiveBufferSize = 33554432;
        _udpClient.Client.SendBufferSize = 33554432;
        Console.WriteLine(_udpClient.Client.ReceiveBufferSize);
        Console.WriteLine(_udpClient.Client.SendBufferSize);
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

    public async Task<UdpReceiveResult> ReadBytesAsync(string fromAddress, int port, int timeoutMs = -1)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(fromAddress), port);
        return await ReadBytesAsync(endpoint, timeoutMs);
    }
    public byte[] ReadBytes(string fromAddress, int port)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(fromAddress), port);
        return ReadBytes(ref endpoint);
    }

    public async Task<UdpReceiveResult> ReadBytesAsync(IPAddress fromAddress, int port, int timeoutMs = -1)
    {
        var endpoint = new IPEndPoint(fromAddress, port);
        return await ReadBytesAsync(endpoint, timeoutMs);
    }
    public byte[] ReadBytes(IPAddress fromAddress, int port)
    {
        var endpoint = new IPEndPoint(fromAddress, port);
        return ReadBytes(ref endpoint);
    }

    public async Task<UdpReceiveResult> ReadBytesAsync(IPEndPoint fromEndPoint = null, int timeoutMs = -1)
    {
        if (UdpClient == null) return new UdpReceiveResult();
        if (timeoutMs < 0) timeoutMs = DefaultTimeoutMilliseconds;

        CancellationTokenSource canceller = new CancellationTokenSource(timeoutMs);
        while (true)
        {
            if (canceller.IsCancellationRequested)
            {
                canceller.Dispose();
                return new UdpReceiveResult();  //CANCELLED
            }
            
            UdpReceiveResult read;
            try
            {
                read = await UdpClient.ReceiveAsync(canceller.Token);
            }
            catch
            {
                canceller.Dispose();
                return new UdpReceiveResult();  //CANCELLED
            }
            

            if (fromEndPoint != null)//ADDY CHECK?
            {
                if (read.RemoteEndPoint.Address.Address != fromEndPoint.Address.Address)
                {
                    continue;
                }
                if (read.RemoteEndPoint.Port != fromEndPoint.Port)
                {
                    continue;
                }
            }

            canceller.Dispose();
            return read;
        }
    }
    public byte[] ReadBytes(ref IPEndPoint fromEndPoint)
    {
        if (UdpClient == null) return null;
        if (fromEndPoint == null) return null;
        var read = UdpClient.Receive(ref fromEndPoint);
        return read;
    }

    public async Task<int> WriteBytesAsync(byte[] bytes, string toAddress, int toPort, int timeoutMs = -1)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(toAddress), toPort);
        return await WriteBytesAsync(bytes, endpoint, timeoutMs);
    }
    public int WriteBytes(byte[] bytes, string toAddress, int toPort)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(toAddress), toPort);
        return WriteBytes(bytes, endpoint);
    }

    public async Task<int> WriteBytesAsync(byte[] bytes, IPAddress toAddress, int toPort, int timeoutMs = -1)
    {
        var endpoint = new IPEndPoint(toAddress, toPort);
        return await WriteBytesAsync(bytes, endpoint, timeoutMs);
    }
    public int WriteBytes(byte[] bytes, IPAddress toAddress, int toPort)
    {
        var endpoint = new IPEndPoint(toAddress, toPort);
        return WriteBytes(bytes, endpoint);
    }

    public async Task<int> WriteBytesAsync(byte[] bytes, IPEndPoint endpoint, int timeoutMs = -1)
    {
        if (UdpClient == null) return 0;
        if (bytes == null) return 0;
        if (endpoint == null) return 0;
        
        try
        {
            return await UdpClient.SendAsync(bytes, bytes.Length, endpoint);//No need to cancel?
        }
        catch
        {
            return 0;
        }
    }
    public int WriteBytes(byte[] bytes, IPEndPoint endpoint)
    {
        if (UdpClient == null) return 0;
        if (bytes == null) return 0;
        if (endpoint == null) return 0;

        int sent = UdpClient.Send(bytes, bytes.Length, endpoint);
        return sent;
    }

    public void Dispose()
    {
        UdpClient?.Dispose();
    }
}
