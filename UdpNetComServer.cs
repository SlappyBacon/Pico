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
        Func<IPEndPoint, byte[], CancellationToken, byte[]> _getRequestReply = null;
        UdpNetCom _com;
        bool _isRunning = false;
        public bool IsRunning { get { return _isRunning; } }

        public UdpNetComServer() { }
        public UdpNetComServer(int port, Func<IPEndPoint, byte[], CancellationToken, byte[]> getRequestReply)
        {
            Start(port, getRequestReply);
        }

        public void Start(int port, Func<IPEndPoint, byte[], CancellationToken, byte[]> getRequestReply)
        {
            if (IsRunning) return;
            _isRunning = true;

            _com = new UdpNetCom(port);
            _cancelTokenSource = new CancellationTokenSource();
            if (getRequestReply == null)
            {
                _getRequestReply = DefaultGetRequestReply;
            }
            else _getRequestReply = getRequestReply;

            Task.Run(() => AcceptRequests(_com, _cancelTokenSource.Token));
        }
        public void Stop()
        {
            if (!IsRunning) return;
            _isRunning = false;

            _cancelTokenSource.Cancel();
            _com.Dispose();
            _cancelTokenSource.Dispose();
        }

        async Task AcceptRequests(UdpNetCom netCom, CancellationToken cancelToken)
        {
            Console.WriteLine("ACCEPTING");
            while (!cancelToken.IsCancellationRequested)
            {
                var nextRequest = await GetNextRequestAsync(netCom, cancelToken);
                if (cancelToken.IsCancellationRequested) break;
                Task.Run(() => ProcessRequest(netCom, nextRequest.Item1, nextRequest.Item2, cancelToken));
            }
            Console.WriteLine("NOT ACCEPTING");
        }

        async Task<(IPEndPoint, ComPacket)> GetNextRequestAsync(UdpNetCom netCom, CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                var read = await netCom.ReadBytesAsync(null, cancelToken);
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
            var replyBytes = _getRequestReply.Invoke(sender, requestPacket.Body, cancelToken);
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
            Stop();
        }

        byte[] DefaultGetRequestReply(IPEndPoint sender, byte[] request, CancellationToken cancelToken)
        {
            //Return reply
            return new byte[] { 40 };
        }
    }
}
