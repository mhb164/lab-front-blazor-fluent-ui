namespace FrontBlazorFluentUI.Services;
public class TimeService : IAsyncDisposable
{
    private System.Timers.Timer _timer;
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

    public ValueTask DisposeAsync()
    {
        _timer?.Dispose();
        return ValueTask.CompletedTask;
    }
}
