namespace Pico.Services;
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

    public bool Start()
    {
        if (IsRunning) return false;
        if (IsStopping) return false;
        isStarting_ = true;

        mainCts_ = new CancellationTokenSource();
        mainTask_ = Main(MainCts.Token);

        isStarting_ = false;
        return true;
    }
    public bool Stop()
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

    public void Dispose()
    {
        Stop();
    }
}
