using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pico.MiscTypes
{
    class FrequencyLimiter
    {
        object _locker = new object();

        readonly int _periodMs;
        readonly int _requestsPerPeriodMax;
        int _requestsThisPeriod;

        public int PeriodMilliseconds { get { return _periodMs; } }
        public int RequestsThisPeriod { get { return _requestsThisPeriod; } }
        public int RequestsPerPeriodMax { get { return _requestsPerPeriodMax; } }
        public bool LimitReached { get { return RequestsThisPeriod == RequestsPerPeriodMax; } }

        public FrequencyLimiter(int maxPerPeriod, int periodMs)
        {
            _periodMs = Math.Clamp(periodMs, 10, int.MaxValue);
            _requestsPerPeriodMax = Math.Clamp(maxPerPeriod, 1, int.MaxValue);
            _requestsThisPeriod = RequestsPerPeriodMax - 1;
            var divided = PeriodMilliseconds / RequestsPerPeriodMax;
            for (int i = 1; i < RequestsPerPeriodMax; i++)
            {
                DecrementAfterPeriod(divided * i);
            }
        }

        public void Next(int timeoutMs = int.MaxValue)
        {
            if (timeoutMs < 1) return;
            CancellationTokenSource cts = new CancellationTokenSource(timeoutMs);
            Next(cts.Token);
            cts.Dispose();
        }
        public void Next(CancellationToken ct)
        {
            lock (_locker)
            {
                while (true)
                {
                    if (!LimitReached) break;
                    if (ct.IsCancellationRequested) return;
                    Thread.Sleep(1);
                }

                _requestsThisPeriod++;
                DecrementAfterPeriod(PeriodMilliseconds, ct);
            }
        }

        async Task DecrementAfterPeriod(int ms)
        {
            await Task.Delay(ms);
            _requestsThisPeriod--;
        }
        async Task DecrementAfterPeriod(int ms, CancellationToken ct)
        {
            await Task.Delay(ms, ct);
            _requestsThisPeriod--;
        }
    }
}
