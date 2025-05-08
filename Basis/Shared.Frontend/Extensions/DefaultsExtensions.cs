using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Services;

namespace Shared.Extensions;

public static class DefaultsExtensions
{
    public static IServiceCollection PrepareDefaults(this IServiceCollection services, ApiClientConfig apiConfig)
    {
        services.AddSingleton(apiConfig);
        services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            PropertyNamingPolicy = null,
            WriteIndented = true,
            IncludeFields = false,
        });

        services.AddSingleton<TimeService>();
        services.AddSingleton<EncryptionService>();
        services.AddSingleton<ICryptoService, CryptoService>();
        
        services.AddScoped<IStorageProvider, StorageProvider>();
        services.AddScoped<IFileDownloadService, FileDownloadService>();
        services.AddScoped<UserContext>();
        services.AddScoped<IAuthStateProvider, AuthStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(provider => (provider.GetRequiredService<IAuthStateProvider>() as AuthStateProvider)!);
        services.AddAuthorizationCore();

        services.AddScoped(sp => new HttpClient());
        services.AddScoped<IApiClient, ApiClient>();
        services.AddScoped<AuthClient>();
        services.AddScoped<LayoutService>();

        return services;
    }
}
