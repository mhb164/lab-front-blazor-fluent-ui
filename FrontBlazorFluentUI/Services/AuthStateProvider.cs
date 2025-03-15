namespace Laboratory.Front.Services;

public class AuthStateProvider : AuthenticationStateProvider, IAuthStateProvider
{
    public static readonly string RefreshTokenHeaderName = "refresh_token";

    private readonly ILogger? _logger;
    private readonly IStorageProvider _storage;
    private readonly IApiClient _apiClient;
    private readonly UserContext _userContext;
    private Timer? _refreshTimer;

    public AuthStateProvider(ILogger<AuthStateProvider>? logger, IStorageProvider storage, IApiClient apiClient, UserContext userContext)
    {
        _logger = logger;
        _storage = storage;
        _apiClient = apiClient;
        _userContext = userContext;

        _storage.AccessTokenChanged += OnAccessTokenChanged;
    }

    public UserContext User => _userContext;

    private UserContext ResetUser(ContextOverview overview, string accessToken)
    {
        _userContext.Reset(overview, accessToken);
        ScheduleTokenRefresh(_userContext.User);
        return _userContext;
    }

    private UserContext ResetUser(ContextOverview? overview)
    {
        _userContext.Reset(overview);
        ScheduleTokenRefresh(_userContext.User);
        return _userContext;
    }

    private UserContext OnServerIsNotAccessible()
    {
        _logger?.LogInformation("AuthStateProvider> On server is not accessible!");
        _userContext.Reset(null);
        return _userContext;
    }
    private void OnAccessTokenChanged()
    {
        _logger?.LogInformation("AuthStateProvider> storage-> AccessTokenChanged");
        NotifyAuthenticationStateChanged();
    }

    public async Task ClearToken()
    {
        _logger?.LogInformation("AuthStateProvider> ClearToken called");
        await _storage.ClearToken();
        _userContext.Reset(null);
        NotifyAuthenticationStateChanged();
    }

    public async Task UpdateToken(Token token)
    {
        _logger?.LogInformation("AuthStateProvider> UpdateToken called");

        await _storage.UpdateToken(token);
        NotifyAuthenticationStateChanged();
    }

    private void ScheduleTokenRefresh(ClientUser? user)
    {
        if (user is null)
        {
            _refreshTimer?.Dispose();
            return;
        }

        var timeUntilExpiry = user.Token.ExpirationTime - DateTime.UtcNow;
        var refreshTime = timeUntilExpiry.TotalSeconds > 60 ? timeUntilExpiry.TotalSeconds - 60 : 30; // Refresh 1 minute before expiry

        _refreshTimer?.Dispose();
        _refreshTimer = new Timer(async _ => await RefreshTokenAsync(), null, (int)(refreshTime * 1000), Timeout.Infinite);
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var overview = User.Overview;
        var overviewResult = default(ServiceResult<ContextOverview>);

        var accessToken = await _storage.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            if (overview is null)
            {
                overviewResult = await _apiClient.GetOverviewAsync();
                if (overviewResult.IsFailed || overviewResult.Value is null)
                    return OnServerIsNotAccessible().AuthenticationState;

                overview = overviewResult.Value!;
            }

            return ResetUser(overview).AuthenticationState;
        }

        overviewResult = await _apiClient.GetOverviewAsync();
        if (overviewResult.IsFailed || overviewResult.Value is null)
            return OnServerIsNotAccessible().AuthenticationState;

        overview = overviewResult.Value!;

        if (!overview.Authenticated)
        {
            var refreshTokenResult = await RefreshTokenAsync();
            if (refreshTokenResult.Code == ServiceResultCode.Unauthorized)
                return ResetUser(overview).AuthenticationState;

            overviewResult = await _apiClient.GetOverviewAsync();
            if (overviewResult.IsFailed || overviewResult.Value is null)
                return OnServerIsNotAccessible().AuthenticationState;

            overview = overviewResult.Value!;
            if (!overview.Authenticated)
                return ResetUser(overview).AuthenticationState;
        }

        accessToken = await _storage.GetAccessTokenAsync();
        return ResetUser(overview, accessToken).AuthenticationState;
    }

    // Call this after login/logout to force a re-check of the authentication state.
    public void NotifyAuthenticationStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    public async Task<ServiceResult> RefreshTokenAsync()
    {
        var refreshToken = await _storage.GetRefreshTokenAsync();
        if (string.IsNullOrWhiteSpace(refreshToken))
            return ServiceResult.Unauthorized();

        var refreshTokenResult = await _apiClient.Post<Token>("/refresh-token", new KeyValuePair<string, string>(RefreshTokenHeaderName, refreshToken));
        if (refreshTokenResult.IsFailed || refreshTokenResult.Value is null)
        {
            await ClearToken();
            return ServiceResult.From(refreshTokenResult);
        }

        await UpdateToken(refreshTokenResult.Value);
        return ServiceResult.Success();
    }
}

