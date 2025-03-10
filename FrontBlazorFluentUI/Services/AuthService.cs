using FrontBlazorFluentUI.Dto;
using Microsoft.AspNetCore.Components.Authorization;

namespace FrontBlazorFluentUI.Services;

public class AuthService
{
    private readonly ILogger? _logger;
    private readonly ApiClient _apiClient;
    private readonly AuthStateProvider _authStateProvider;

    public AuthService(ILogger<AuthService>? logger, ApiClient apiClient, AuthenticationStateProvider authStateProvider)
    {
        _logger = logger;
        _apiClient = apiClient;
        _authStateProvider = authStateProvider as AuthStateProvider;
    }

    public async Task<ServiceResult> SignInAsync(SignInRequest request)
    {
        var signInResult = await _apiClient.Post<SignInRequest, TokenDto>("/sign-in", request);
        if (signInResult.IsFailed || signInResult.Value is null)
        {
            await _authStateProvider.ClearToken();
            return ServiceResult.From(signInResult);
        }

        await _authStateProvider.UpdateToken(signInResult.Value);
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


}