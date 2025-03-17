namespace Shared.Config;

public class ApiClientConfig
{
    public readonly string BaseAddress;

    public ApiClientConfig(string? baseAddress)
    {
        if (string.IsNullOrWhiteSpace(baseAddress))
            throw new ArgumentNullException(nameof(baseAddress));

        BaseAddress = baseAddress;
    }
}
