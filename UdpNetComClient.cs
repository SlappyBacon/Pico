using Pico.Arrays;
using Pico.Networking;
using Pico.Randoms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pico.Networking
{
    class UdpNetComClient : IDisposable
    {
        object _comWriteLock = new object();

        CancellationTokenSource _cancelTokenSource;
        
        UdpNetCom _com;
        bool _isRunning = false;
        public bool IsRunning { get { return _isRunning; } }


        int _nextRequestId = 0;
        List<ComPacket> _comReplies = new List<ComPacket>();
        public int DebugComRepliesCount { get { return _comReplies.Count; } }

        public UdpNetComClient() { }
        public UdpNetComClient(int port)
        {
            Start(port);
        }

        
        public void Start(int port)
        {
            if (IsRunning) return;
            _isRunning = true;

            _cancelTokenSource = new CancellationTokenSource();
            _com = new UdpNetCom(port);
            Task.Run(() => AcceptReplies(_com, _cancelTokenSource.Token));
        }
        
        public void Stop()
        {
            if (!IsRunning) return;
            _isRunning = false;

            _cancelTokenSource.Cancel();
            _com.Dispose();
            _cancelTokenSource.Dispose();
        }




        public async Task<byte[]> SendRequest(IPEndPoint serverEndPoint, byte[] request)
        {
            if (_cancelTokenSource.IsCancellationRequested) return new byte[0];
            var requestPacket = new ComPacket(_nextRequestId++, request);
            var requestPacketBytes = requestPacket.ToBytes();
            lock (_comWriteLock)
            {
                _com.WriteBytes(requestPacketBytes, serverEndPoint);
            }
            //Console.WriteLine($"[{DateTime.Now}] TX {requestPacket.Prefix}:{ArrayTools.ToString(requestPacket.Body)}");
            var replyPacket = await WaitForReplyInList(requestPacket.Prefix, _cancelTokenSource.Token);
            return replyPacket.Body;
        }

        async Task AcceptReplies(UdpNetCom netCom, CancellationToken cancelToken)
        {
            Console.WriteLine("ACCEPTING");
            while (!cancelToken.IsCancellationRequested)
            {
                var nextReply = await GetNextReplyAsync(netCom, cancelToken);
                if (cancelToken.IsCancellationRequested) break;
                AddReplyToList(netCom, nextReply);
            }
            Console.WriteLine("NOT ACCEPTING");
        }

        async Task<ComPacket> GetNextReplyAsync(UdpNetCom netCom, CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                var read = await netCom.ReadBytesAsync(null, cancelToken);
                if (read.RemoteEndPoint == null) continue;
                ComPacket replyPacket = ComPacket.FromBytes(read.Buffer);
                return replyPacket;
            }
            return new ComPacket(-1);
        }

        void AddReplyToList(UdpNetCom netCom, ComPacket replyPacket)
        {
            //Console.WriteLine($"[{DateTime.Now}] RX {replyPacket.Prefix}:{ArrayTools.ToString(replyPacket.Body)}");

            lock (_comReplies)
            {
                _comReplies.Add(replyPacket);
            }
        }

        async Task<ComPacket> WaitForReplyInList(int requestId, CancellationToken cancelToken)
        {
            DateTime timeoutTime = DateTime.Now + TimeSpan.FromMilliseconds(10000);
            while (DateTime.Now < timeoutTime && !cancelToken.IsCancellationRequested)
            {
                lock (_comReplies)
                {
                    for (int i = 0; i < _comReplies.Count; i++)
                    {
                        if (_comReplies[i].Prefix != requestId) continue;
                        var result = _comReplies[i];
                        _comReplies.RemoveAt(i);
                        return result;
                    }
                }
                await Task.Delay(1);
            }
            return new ComPacket(-1);
        }

        public void Dispose()
        {
            Stop();
        }

        public async Task DebugFloodRequests(IPEndPoint serverEndPoint, byte[] request, int count = 1)
        {
            if (count < 1) return;
            Task<byte[]>[] wave = new Task<byte[]>[count];
            for (int i = 0; i < wave.Length; i++)
            {
                if (request == null)
                {
                    //RANDOM
                    byte[] rng = new byte[10000];
                    Random.Shared.NextBytes(rng);
                    wave[i] = SendRequest(serverEndPoint, rng);
                }
                else
                {
                    //FIXED
                    wave[i] = SendRequest(serverEndPoint, request);
                }
            }
            for (int i = 0; i < wave.Length; i++)
            {
                await wave[i];
                wave[i].Dispose();
            }
            wave = null;
        }
    }
}
