namespace Shared.Services;

public class AuthClient
{
    private readonly ILogger? _logger;
    private readonly IApiClient _apiClient;
    private readonly IAuthStateProvider _authStateProvider;

    public AuthClient(ILogger<AuthClient>? logger, IApiClient apiClient, IAuthStateProvider authStateProvider)
    {
        _logger = logger;
        _apiClient = apiClient;
        _authStateProvider = authStateProvider ?? throw new ArgumentNullException(nameof(authStateProvider));
    }

    public async Task<ServiceResult<SignInOptions>> GetSignInOptionsAsync()
    {
        var serviceResult = await _apiClient.Get<SignInOptions>("/sign-in-options");
        return serviceResult;
    }
   
    public async Task<ServiceResult> SignInAsync(SignInRequest request)
    {
        var signInResult = await _apiClient.Post<SignInRequest, Token>("/sign-in", request);
        if (signInResult.IsFailed || signInResult.Value is null)
        {
            await _authStateProvider.ClearToken();
            return ServiceResult.From(signInResult);
        }

        await _authStateProvider.UpdateToken(signInResult.Value, true);
        return ServiceResult.Success();
    }

    public async Task<ServiceResult> ChangePasswordAsync(ChangeLocalPasswordRequest request)
    {
        var result = await _apiClient.Post("/change-local-password", request);
        if (result.IsFailed)
        {
            return result;
        }

        await _authStateProvider.RefreshTokenAsync();
        return ServiceResult.Success();
    }

    public async Task SignOutAsync()
    {
        try
        {
            await _apiClient.Post("/sign-out");
        }
        catch { }
        await _authStateProvider.ClearToken();
    }

    public async Task<ServiceResult<UserInfo>> GetUserInfoAsync()
    {
        var serviceResult = await _apiClient.Get<UserInfo>("/user-info");
        return serviceResult;
    }

}