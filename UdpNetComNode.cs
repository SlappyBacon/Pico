using Newtonsoft.Json;
using Pico.Logger;
using Pico.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Networking;

class UdpNetComNode : IDisposable
{
    CancellationTokenSource _mainCts;
    CancellationTokenSource MainCTS => _mainCts;

    UdpNetCom _com;
    UdpNetCom COM => _com;

    Task _readerTask;
    Task ReaderTask => _readerTask;
    Action<UdpNetComNode, UdpReceiveResult, CancellationToken> _onBytesRead = null;
    Action<UdpNetComNode, UdpReceiveResult, CancellationToken> OnBytesRead => _onBytesRead;
    public void SubscribeOnBytesRead(Action<UdpNetComNode, UdpReceiveResult, CancellationToken> action) => _onBytesRead += action;
    public void UnSubscribeOnBytesRead(Action<UdpNetComNode, UdpReceiveResult, CancellationToken> action) => _onBytesRead -= action;

    public UdpNetComNode(int port)
    {
        _com = new UdpNetCom(port, UdpNetCom.MaxBufferSize);
        _mainCts = new CancellationTokenSource();
        _readerTask = ReaderTaskAction(MainCTS.Token);
    }

    //Auto Read :)
    async Task ReaderTaskAction(CancellationToken ct)
    {
        //Console.WriteLine("READ START");
        while (true)
        {
            if (ct.IsCancellationRequested) break;
            var read = await COM.ReadBytesAsync(ct);
            if (ct.IsCancellationRequested) break;
            if (read.Buffer == null) continue;
            Task.Run(() => OnBytesRead?.Invoke(this, read, ct));
        }
        //Console.WriteLine("READ END");
    }

    //Manual Write :)
    public async Task<int> WriteBytesAsync(byte[] bytes, string toAddress, int toPort) => await COM.WriteBytesAsync(bytes, toAddress, toPort);
    public async Task<int> WriteBytesAsync(byte[] bytes, IPAddress toAddress, int toPort) => await COM.WriteBytesAsync(bytes, toAddress, toPort);
    public async Task<int> WriteBytesAsync(byte[] bytes, IPEndPoint toEndPoint) => await COM.WriteBytesAsync(bytes, toEndPoint);


    public void Dispose()
    {
        MainCTS.Cancel();

        //Wait(s) & Dispose(s)

        ReaderTask.Wait();
        ReaderTask.Dispose();
        _onBytesRead = null;

        COM.Dispose();

        MainCTS.Dispose();
    }
}
