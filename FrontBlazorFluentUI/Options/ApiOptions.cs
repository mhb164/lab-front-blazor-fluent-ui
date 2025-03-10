using FrontBlazorFluentUI.Model;

namespace FrontBlazorFluentUI.Options;

public class ApiOptions
{
    public const string ConfigName = "Api";

    public string? BaseAddress { get; set; }

    public static ApiClientConfig ToModel(ApiOptions? options)
    {
        return options is null 
            ? throw new ArgumentNullException(nameof(options)) 
            : new ApiClientConfig(options.BaseAddress);
    }
}
