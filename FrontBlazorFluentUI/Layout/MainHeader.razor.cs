using FrontBlazorFluentUI.Components;
using FrontBlazorFluentUI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace FrontBlazorFluentUI.Layout;

public sealed partial class MainHeader : IDisposable
{
    [Inject] private LayoutService LayoutService { get; set; } = default!;

    protected override void OnInitialized()
    {
        LayoutService.OnChange += StateHasChanged; // Listen for changes
    }

    public void Dispose()
    {
        LayoutService.OnChange -= StateHasChanged; // Clean up listener
    }
}
