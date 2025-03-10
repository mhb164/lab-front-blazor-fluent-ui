namespace FrontBlazorFluentUI.Dto;

public class ContextOverview
{
    public DateTime Now { get; set; }
    public DateTime UtcNow { get; set; }
    public string ProductName { get; set; }
    public string Version { get; set; }
    public string? ClientAddress { get; set; }
    public bool Authenticated { get; set; }

    public DateTime ReceivedAtUtc { get; private set; }
    public TimeSpan LifeTime => DateTime.UtcNow - ReceivedAtUtc;

    public void SetReceivedTime()
    {
        ReceivedAtUtc = DateTime.UtcNow;
    }

}
