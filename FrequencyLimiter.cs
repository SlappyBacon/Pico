namespace Pico.MiscTypes
{
    /// <summary>
    /// A tool for limiting the frequency of
    /// things.  Example: API calls per minute.
    /// </summary>
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

        CancellationTokenSource _cts = new CancellationTokenSource();
        CancellationTokenSource CTS => _cts;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="maxPerPeriod">Maximum Next() calls per period.</param>
        /// <param name="periodMs">Period time in milliseconds.</param>
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
        /// <summary>
        /// Waits for next available time,
        /// then returns.
        /// </summary>
        public void Next()
        {
            lock (_locker)
            {
                while (true)
                {
                    if (!LimitReached) break;
                    if (CTS.IsCancellationRequested) return;
                    Thread.Sleep(1);
                }

                _requestsThisPeriod++;
                DecrementAfterPeriod(PeriodMilliseconds);
            }
        }

        /// <summary>
        /// Waits for next available time,
        /// then returns.
        /// </summary>
        /// <param name="externalCancelToken">External, additional cancel token.</param>
        public void Next(CancellationToken externalCancelToken)
        {
            lock (_locker)
            {
                while (true)
                {
                    if (!LimitReached) break;
                    if (CTS.IsCancellationRequested) return;
                    if (externalCancelToken.IsCancellationRequested) return;
                    Thread.Sleep(1);
                }

                _requestsThisPeriod++;
                DecrementAfterPeriod(PeriodMilliseconds);
            }
        }

        async Task DecrementAfterPeriod(int ms)
        {
            await Task.Delay(ms, CTS.Token);
            _requestsThisPeriod--;
        }
    }
}
