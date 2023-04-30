using Pico.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pico.Networking
{
    public class UdpNetComServer : IDisposable
    {
        object _comWriteLock = new object();
        CancellationTokenSource _cancelTokenSource;
        Func<byte[], CancellationToken, byte[]> _getRequestReply = null;
        UdpNetCom _com;

        public UdpNetComServer(int port, Func<byte[], CancellationToken, byte[]> getRequestReply)
        {
            _com = new UdpNetCom(port);

            _cancelTokenSource = new CancellationTokenSource();

            if (getRequestReply == null)
            {
                _getRequestReply = DefaultGetRequestReply;
            }
            else _getRequestReply = getRequestReply;

            Task.Run(() => AcceptRequests(_com, _cancelTokenSource.Token));
        }

        async Task AcceptRequests(UdpNetCom netCom, CancellationToken cancelToken)
        {
            Console.WriteLine("ACCEPTING");
            while (!cancelToken.IsCancellationRequested)
            {
                var nextRequest = await GetNextRequestAsync(netCom, cancelToken);
                if (cancelToken.IsCancellationRequested) break;
                var worker = Task.Run(() => ProcessRequest(netCom, nextRequest.Item1, nextRequest.Item2, cancelToken));
            }
            Console.WriteLine("NOT ACCEPTING");
        }

        async Task<(IPEndPoint, ComPacket)> GetNextRequestAsync(UdpNetCom netCom, CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                var read = await netCom.ReadBytesAsync(null, 1000);
                if (read.RemoteEndPoint == null) continue;
                ComPacket requestPacket = ComPacket.FromBytes(read.Buffer);
                return (read.RemoteEndPoint, requestPacket);
            }
            return (null, new ComPacket(-1));
        }

        void ProcessRequest(UdpNetCom netCom, IPEndPoint sender, ComPacket requestPacket, CancellationToken cancelToken)
        {
            //Console.WriteLine($"[{DateTime.Now}] [{sender.Address}:{sender.Port}] RX {requestPacket.Prefix}:{ArrayTools.ToString(requestPacket.Body)}");
            //Process request => reply bytes\\
            if (cancelToken.IsCancellationRequested) return;
            var replyBytes = _getRequestReply.Invoke(requestPacket.Body, cancelToken);
            var replyPacket = new ComPacket(requestPacket.Prefix, replyBytes);
            
            if (cancelToken.IsCancellationRequested) return;
            ReplyToSender(netCom, sender, replyPacket);//DEBUG ECHO REPLY :)
        }

        void ReplyToSender(UdpNetCom netCom, IPEndPoint sender, ComPacket replyPacket)
        {
            //Console.WriteLine($"[{DateTime.Now}] [{sender.Address}:{sender.Port}] TX {replyPacket.Prefix}:{ArrayTools.ToString(replyPacket.Body)}");
            var replyPacketBytes = replyPacket.ToBytes();

            lock (_comWriteLock)
            {
                netCom.WriteBytes(replyPacketBytes, sender);
            }
        }

        public void Dispose()
        {
            Console.WriteLine("server disposing...");
            _cancelTokenSource.Cancel();
            _com.Dispose();
            _cancelTokenSource.Dispose();
        }

        byte[] DefaultGetRequestReply(byte[] request, CancellationToken cancelToken)
        {
            //Return reply
            return new byte[] { 40 };
        }
    }
}
