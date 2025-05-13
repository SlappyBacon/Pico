using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    abstract class Service : IDisposable
    {
        CancellationTokenSource mainCts_ = null;
        CancellationTokenSource MainCts => mainCts_;

        Task mainTask_ = null;
        Task MainTask => mainTask_;

        public bool IsRunning => MainTask != null && !MainTask.IsCompleted;

        bool isStarting_ = false;
        public bool IsStarting => isStarting_;

        bool isStopping_ = false;
        public bool IsStopping => isStopping_;

        protected abstract Task Main(CancellationToken cancelToken);

        public async Task<bool> Start()
        {
            if (IsRunning) return false;
            if (IsStopping) return false;
            isStarting_ = true;
            mainCts_ = new CancellationTokenSource();
            mainTask_ = Main(MainCts.Token);
            isStarting_ = false;
            return true;
        }
        public async Task<bool> Stop()
        {
            if (!IsRunning) return false;
            if (IsStarting) return false;
            isStopping_ = true;
            MainCts.Cancel();
            MainTask.Wait();
            MainTask.Dispose();
            MainCts.Dispose();
            isStopping_ = false;
            return true;
        }

        public async void Dispose()
        {
            await Stop();
        }
    }
}
