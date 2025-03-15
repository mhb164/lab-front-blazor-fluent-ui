var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration
var apiOptions = builder.Configuration.GetSection(ApiOptions.ConfigName)?.Get<ApiOptions>();
builder.Services.AddSingleton(ApiOptions.ToModel(apiOptions));//ApiClientConfig

// Services
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = false,
    PropertyNamingPolicy = null,
    WriteIndented = true,
    IncludeFields = false,
});

builder.Services.AddSingleton<TimeService>();
builder.Services.AddScoped<IStorageProvider, StorageProvider>();
builder.Services.AddScoped<UserContext>();
builder.Services.AddScoped<IAuthStateProvider, AuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => (provider.GetRequiredService<IAuthStateProvider>() as AuthStateProvider)!);
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddScoped<AuthClient>();
builder.Services.AddScoped<LayoutService>();

builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
