namespace Laboratory.Front.Services;

public sealed class TimeService : IDisposable
{
    private readonly System.Timers.Timer _timer;
    public event Action<DateTime>? OnTimeChanged;
    public DateTime CurrentTime { get; private set; } = DateTime.Now;

    public TimeService()
    {
        _timer = new System.Timers.Timer(500); 
        _timer.Elapsed += (sender, args) =>
        {
            CurrentTime = DateTime.Now;
            OnTimeChanged?.Invoke(CurrentTime);
        };
        _timer.Start();
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
