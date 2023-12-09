using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Pico.Networking
{
    class ComClient : IDisposable
    {
        TimeSpan _timeoutTime = TimeSpan.FromSeconds(2);
        UdpNetCom _com;
        CancellationTokenSource _cts;
        Task _reader;
        List<(DateTime UtcReceiveTime, ComPacket Packet)> _replies = new List<(DateTime UtcReceiveTime, ComPacket Packet)>();

        TimeSpan TimeoutTime { get {  return _timeoutTime; } }
        UdpNetCom Com => _com;
        CancellationTokenSource CTS => _cts;
        Task ReaderTask => _reader;
        List<(DateTime UtcReceiveTime, ComPacket Packet)> Replies { get { return _replies; } }


        public ComClient(int port)
        {
            _com = new UdpNetCom(port);
            _cts = new CancellationTokenSource();
            _reader = Task.Run(() => ReadTaskMainAsync(), CTS.Token);
        }

        async Task ReadTaskMainAsync()
        {
            //Console.WriteLine("CLI: Main Started.");
            while (true)
            {
                if (CTS.IsCancellationRequested) break;
                var read = await Com.ReadBytesAsync(null, CTS.Token);
                var packet = ComPacket.FromBytes(read.Buffer);
                //Console.WriteLine($"CLI: <= {packet}");
                lock (Replies)
                {
                    Replies.Add((DateTime.Now, packet));
                }
            }
            //Console.WriteLine("CLI: Main Stopped.");
        }

        public async Task<byte[]> RequestAsync(IPEndPoint serverEndPoint, byte[] body)
        {
            //Send Packet
            var packet = new ComPacket(body);
            //Console.WriteLine($"CLI: => {packet}");
            await Com.WriteBytesAsync(packet.ToBytes(), serverEndPoint);
            var waiter = WaitForReply(packet.GUID);
            packet = ComPacket.Empty;   //Free Memory
            return await waiter;
        }
        async Task<byte[]> WaitForReply(byte[] GUID)
        {
            DateTime utcTimeoutTime = DateTime.UtcNow + TimeoutTime;

            byte[] result = null;

            //Wait for reply
            while (true)
            {
                if (CTS.Token.IsCancellationRequested) break;
                if (DateTime.Now > utcTimeoutTime) break;

                //Console.WriteLine($"CLI: WAIT TICK ({ArrayTools.ToString(GUID)})");

                lock (Replies)
                {
                    for (int i = 0; i < Replies.Count; i++)
                    {
                        if (GUID.Equals(Replies[i].Packet.GUID)) continue;
                        //Found
                        //Console.WriteLine($"CLI: FOUND");
                        result = Replies[i].Packet.Body;
                        Replies.RemoveAt(i);
                        break;
                    }
                }
                if (result != null) break;
                
                //Keep waiting
                await Task.Delay(1, CTS.Token);
            }

            return result;
        }
        public byte[] Request(IPEndPoint serverEndPoint, byte[] body)
        {
            var reqTask = RequestAsync(serverEndPoint, body);
            reqTask.Wait();
            return reqTask.Result;
        }

        public void Cleanup()
        {
            //Expired by at least a short time.
            DateTime utcEraseBeforeTime = DateTime.UtcNow - TimeoutTime - TimeSpan.FromSeconds(1);

            List<int> foundIndeces = new List<int>();
            lock (Replies)
            {
                //Find All Indeces
                for (int i = 0; i < Replies.Count; i++)
                {
                    if (utcEraseBeforeTime < Replies[i].UtcReceiveTime) continue;
                    foundIndeces.Add(i);
                }

                //Remove From Replies
                for (int i = foundIndeces.Count-1; i >= 0; i--)
                {
                    Replies.RemoveAt(i);
                }
            }
        }

        public async Task FloodAsync(IPEndPoint serverEndPoint, byte[] body, int count)
        {
            int totalFinished = 0;
            for (int i = 0; i < count; i++)
            {
                PerformOne();
            }

            while (totalFinished < count)
            {
                if (CTS.IsCancellationRequested) break;
                await Task.Delay(1, CTS.Token);
            }

            async Task PerformOne()
            {
                await RequestAsync(serverEndPoint, body);
                totalFinished++;
            }
        }
        public void Dispose()
        {
            CTS.Cancel();
            ReaderTask.Wait();
            CTS.Dispose();
            Com.Dispose();
            Replies.Clear();
        }
    }
}
