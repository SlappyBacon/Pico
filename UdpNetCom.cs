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
    public const int MinBufferSize = 1048;          //1kB
    public const int MaxBufferSize = 1073741824;    //1GB
    public UdpNetCom(int myLocalPort, int rxBufferSize, int txBufferSize) //Where I receive messages
    {
        _udpClient = new UdpClient(myLocalPort);
        _udpClient.Client.ReceiveBufferSize = Math.Clamp(rxBufferSize, MinBufferSize, MaxBufferSize);
        _udpClient.Client.SendBufferSize = Math.Clamp(txBufferSize, MinBufferSize, MaxBufferSize);
    }
    public UdpNetCom(int myLocalPort, int bufferSize = 1048576) : this(myLocalPort, bufferSize, bufferSize) { }



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
        if (UdpClient == null) return null;
        if (fromEndPoint == null) return null;
        var read = UdpClient.Receive(ref fromEndPoint);
        return read;
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

    public int WriteBytes(byte[] bytes, string toAddress, int toPort)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(toAddress), toPort);
        return WriteBytes(bytes, endpoint);
    }
    public int WriteBytes(byte[] bytes, IPAddress toAddress, int toPort)
    {
        var endpoint = new IPEndPoint(toAddress, toPort);
        return WriteBytes(bytes, endpoint);
    }
    public int WriteBytes(byte[] bytes, IPEndPoint endpoint)
    {
        if (UdpClient == null) return 0;
        if (bytes == null) return 0;
        if (endpoint == null) return 0;

        int sent = UdpClient.Send(bytes, bytes.Length, endpoint);
        return sent;
    }


    public async Task<UdpReceiveResult> ReadBytesAsync(int timeoutMs) => await ReadBytesAsync(null, timeoutMs);
    public async Task<UdpReceiveResult> ReadBytesAsync(CancellationToken cancelToken) => await ReadBytesAsync(null, cancelToken);
    public async Task<UdpReceiveResult> ReadBytesAsync(IPAddress fromAddress, int port, int timeoutMs = -1)
    {
        var endpoint = new IPEndPoint(fromAddress, port);
        return await ReadBytesAsync(endpoint, timeoutMs);
    }
    public async Task<UdpReceiveResult> ReadBytesAsync(IPAddress fromAddress, int port, CancellationToken cancelToken)
    {
        var endpoint = new IPEndPoint(fromAddress, port);
        return await ReadBytesAsync(endpoint, cancelToken);
    }
    public async Task<UdpReceiveResult> ReadBytesAsync(string fromAddress, int port, int timeoutMs = -1)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(fromAddress), port);
        return await ReadBytesAsync(endpoint, timeoutMs);
    }
    public async Task<UdpReceiveResult> ReadBytesAsync(string fromAddress, int port, CancellationToken cancelToken)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(fromAddress), port);
        return await ReadBytesAsync(endpoint, cancelToken);
    }
    public async Task<UdpReceiveResult> ReadBytesAsync(IPEndPoint fromAddress = null, int timeoutMs = -1)
    {
        if (timeoutMs < 0) timeoutMs = DefaultTimeoutMilliseconds;
        CancellationTokenSource canceller = new CancellationTokenSource(timeoutMs);
        var result = await ReadBytesAsync(fromAddress, canceller.Token);
        canceller.Dispose();
        return result;
    }
    public async Task<UdpReceiveResult> ReadBytesAsync(IPEndPoint fromAddress, CancellationToken cancelToken)
    {
        if (UdpClient == null) return new UdpReceiveResult();

        while (true)
        {
            if (cancelToken.IsCancellationRequested)
            {
                return new UdpReceiveResult();  //CANCELLED
            }

            UdpReceiveResult read;
            try
            {
                read = await UdpClient.ReceiveAsync(cancelToken);
            }
            catch
            {
                return new UdpReceiveResult();  //CANCELLED
            }


            if (fromAddress != null)//ADDY CHECK?
            {
                if (read.RemoteEndPoint.Address.Address != fromAddress.Address.Address)
                {
                    continue;
                }
                if (read.RemoteEndPoint.Port != fromAddress.Port)
                {
                    continue;
                }
            }

            return read;
        }
    }
    public async Task<int> WriteBytesAsync(byte[] bytes, string toAddress, int toPort)
    {
        var endpoint = new IPEndPoint(IPAddress.Parse(toAddress), toPort);
        return await WriteBytesAsync(bytes, endpoint);
    }
    public async Task<int> WriteBytesAsync(byte[] bytes, IPAddress toAddress, int toPort)
    {
        var endpoint = new IPEndPoint(toAddress, toPort);
        return await WriteBytesAsync(bytes, endpoint);
    }
    public async Task<int> WriteBytesAsync(byte[] bytes, IPEndPoint endpoint)
    {
        if (UdpClient == null) return 0;
        if (bytes == null) return 0;
        if (endpoint == null) return 0;
        
        try
        {
            return await UdpClient.SendAsync(bytes, bytes.Length, endpoint);
        }
        catch
        {
            return 0;
        }
    }

    public void Dispose()
    {
        UdpClient?.Dispose();
    }
}
