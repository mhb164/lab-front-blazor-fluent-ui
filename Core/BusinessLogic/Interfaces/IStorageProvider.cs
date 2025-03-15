namespace Laboratory.Front.Interfaces;

public interface IStorageProvider
{
    Task UpdateToken(Token token);
    Task ClearToken();
    Task<string> GetAccessTokenAsync();
    Task<string> GetRefreshTokenAsync();

    event Action? AccessTokenChanged;
}

