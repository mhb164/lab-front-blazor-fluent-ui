namespace Laboratory.Front.Layout;

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
