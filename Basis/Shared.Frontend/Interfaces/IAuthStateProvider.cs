namespace Shared.Interfaces;

public interface IAuthStateProvider
{
    Task ClearToken();
    Task UpdateToken(Token token, bool notify);
    Task<ServiceResult> RefreshTokenAsync();
}