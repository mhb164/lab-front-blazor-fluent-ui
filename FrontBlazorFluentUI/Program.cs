using Laboratory.Frontend.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration
var apiOptions = builder.Configuration.GetSection(ApiOptions.ConfigName)?.Get<ApiOptions>();
var apiConfig = ApiOptions.ToModel(apiOptions);
builder.Services.PrepareDefaults(apiConfig);

builder.Services.AddScoped<FileShareClient>();
builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
