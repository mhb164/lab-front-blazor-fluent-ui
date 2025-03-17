namespace Shared.Interfaces;

public interface IAuthStateProvider
{
    Task ClearToken();
    Task UpdateToken(Token value);
    Task<ServiceResult> RefreshTokenAsync();
}