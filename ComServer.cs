using Pico.Arrays;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Pico.Networking
{
    class ComServer : IDisposable
    {
        UdpNetCom _com;
        CancellationTokenSource _cts;
        Func<IPEndPoint, byte[], CancellationToken, byte[]> _serveAction;
        Task _reader;

        UdpNetCom Com => _com;
        CancellationTokenSource CTS => _cts;
        Func<IPEndPoint, byte[], CancellationToken, byte[]> ServeAction => _serveAction;
        Task ReaderTask => _reader;

        ulong _totalRequestsHandled = 0;
        public ulong TotalRequestsHandled => _totalRequestsHandled;

        public ComServer(int port, Func<IPEndPoint, byte[], CancellationToken, byte[]> serveAction)
        {
            _com = new UdpNetCom(port);
            _cts = new CancellationTokenSource();
            _serveAction = serveAction;
            _reader = Task.Run(() => ReadTaskMainAsync(), CTS.Token);
        }

        async Task ReadTaskMainAsync()
        {
            //Console.WriteLine("SRV: Main Started.");
            while (true)
            {
                if (CTS.IsCancellationRequested) break;
                var read = await Com.ReadBytesAsync(null, CTS.Token);
                //Process on a new thread
                Task.Run(() => ProcessRequestAsync(read));
            }
            //Console.WriteLine("SRV: Main Stopped.");
        }
        
        async Task ProcessRequestAsync(UdpReceiveResult request)
        {
            var packet = ComPacket.FromBytes(request.Buffer);

            if (CTS.IsCancellationRequested) return;
            if (ServeAction == null) return;

            
            //Console.WriteLine($"RX: [{request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port}] {ArrayTools.ToString(packet.GUID)} {ArrayTools.ToString(packet.Body)}");
            var replyBytes = ServeAction.Invoke(request.RemoteEndPoint, packet.Body, CTS.Token);
            if (replyBytes == null) return;
            packet.Body = replyBytes;
            await Com.WriteBytesAsync(packet.ToBytes(), request.RemoteEndPoint);
            _totalRequestsHandled++;

            Console.WriteLine($"[{TotalRequestsHandled}][{request.RemoteEndPoint.Address}:{request.RemoteEndPoint.Port}]{ArrayTools.ToString(packet.GUID)} {ArrayTools.ToString(packet.Body)}");
        }

        public void Dispose()
        {
            CTS.Cancel();
            ReaderTask.Wait();
            CTS.Dispose();
            Com.Dispose();
            _serveAction = null;
        }

    }
}
