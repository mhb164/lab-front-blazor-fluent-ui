namespace Shared.Services;

public class StorageProvider : IStorageProvider
{
    private const string AccessTokenKey = "accessToken";
    private const string RefreshTokenKey = "refreshToken";

    private readonly ILogger? _logger;
    private readonly IJSRuntime _jsRuntime;

    private static event Action? _staticAccessTokenChanged;
    public event Action? AccessTokenChanged;
    public StorageProvider(ILogger<StorageProvider>? logger, IJSRuntime jsRuntime)
    {
        _logger = logger;
        _jsRuntime = jsRuntime;

        _staticAccessTokenChanged += OnStaticAccessTokenChanged;
        ListenForAuthChanges();
    }


    public async Task UpdateToken(Token token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, token.AccessToken);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, token.RefreshToken);
    }

    public async Task ClearToken()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
    }

    public async Task<string> GetAccessTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AccessTokenKey);
    }

    public async Task<string> GetRefreshTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", RefreshTokenKey);
    }


    private void OnStaticAccessTokenChanged()
    {
        AccessTokenChanged?.Invoke();
    }

    [JSInvokable]
    public static async Task NotifyStaticAccessTokenChanged()
    {
        _staticAccessTokenChanged?.Invoke();
        await Task.CompletedTask;
    }

    private void ListenForAuthChanges()
    {
        Task.Run(async () => await _jsRuntime.InvokeVoidAsync("eval", $@"
            window.addEventListener('storage', (event) => {{
                if (event.key === '{AccessTokenKey}') {{
                    DotNet.invokeMethodAsync('Laboratory.Shared.Frontend', '{nameof(NotifyStaticAccessTokenChanged)}');
                }}
            }});
        "));
    }
}

