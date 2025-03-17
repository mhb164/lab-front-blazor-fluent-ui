namespace Shared.Model;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public DateTime Time { get; set; }
    public Guid? TrackingId { get; set; }
    public string? Message { get; set; }
}
