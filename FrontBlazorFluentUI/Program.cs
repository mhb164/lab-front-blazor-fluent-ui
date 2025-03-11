using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using FrontBlazorFluentUI;
using FrontBlazorFluentUI.Services;
using System.Text.Json;
using FrontBlazorFluentUI.Options;
using Microsoft.AspNetCore.Components.Authorization;
using FrontBlazorFluentUI.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration
var apiOptions = builder.Configuration.GetSection(ApiOptions.ConfigName)?.Get<ApiOptions>();
builder.Services.AddSingleton(ApiOptions.ToModel(apiOptions));

// Services
builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = false,
    PropertyNamingPolicy = null,
    WriteIndented = true,
});
builder.Services.AddSingleton<TimeService>();
builder.Services.AddScoped<LayoutService>();
builder.Services.AddScoped<IStorageProvider, StorageProvider>();
builder.Services.AddScoped<UserContext>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
