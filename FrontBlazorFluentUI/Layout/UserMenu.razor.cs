﻿namespace Laboratory.Front.Layout;

public sealed partial class UserMenu : IDisposable
{
    private static readonly IEnumerable<OfficeColor?> AllOfficeColors = Enum.GetValues<OfficeColor>().Select(i => (OfficeColor?)i);

    [Inject] public required IDialogService DialogService { get; set; } = default!;
    [Inject] public required AuthClient AuthClient { get; set; } = default!;
    [Inject] private UserContext UserContext { get; set; } = default!;

    private bool _userMenuVisible = false;
    private bool _serverAccessible = false;
    private string? _userMenuTitle = string.Empty;


    private static string? GetCustomColor(OfficeColor? color)
    {
        return color switch
        {
            null => OfficeColorUtilities.GetRandom(true).ToAttributeValue(),
            Microsoft.FluentUI.AspNetCore.Components.OfficeColor.Default => "#036ac4",
            _ => color.ToAttributeValue(),
        };

    }

    protected override async Task OnInitializedAsync()
    {
        UserContext.UserChanged += OnUserChanged;
        UpdateUI(true);
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        UserContext.UserChanged -= OnUserChanged;
    }

    private void OnUserChanged() => UpdateUI();

    private void UpdateUI(bool onInitialized = false)
    {
        _serverAccessible = UserContext.ServerAccessible;
        _userMenuTitle = UserContext.Title;

        if (onInitialized)
            return;
        InvokeAsync(StateHasChanged);
    }

    private void OnUserMenuClick()
    {
        _userMenuVisible = !_userMenuVisible;
    }

    public DesignThemeModes Mode { get; set; }
    public OfficeColor? OfficeColor { get; set; } = Microsoft.FluentUI.AspNetCore.Components.OfficeColor.Default;
   
    private async Task OnSwitchThemeClickAsync()
    {
        if (Mode != DesignThemeModes.Dark)
            Mode = DesignThemeModes.Dark;
        else
            Mode = DesignThemeModes.Light;

        await Task.CompletedTask;
    }

    private async Task OnSignInClickAsync()
    {
        var dialog = await DialogService.ShowDialogAsync<SignInPanel>(new DialogParameters());
        await dialog.Result;
    }

    private async Task OnChangeLocalPasswordAsync()
    {
        var dialog = await DialogService.ShowDialogAsync<ChangeLocalPasswordPanel>(new DialogParameters());
        await dialog.Result;
    }

    private async Task OnSignOutAsync()
    {
        var dialog = await DialogService.ShowConfirmationAsync("Are you <strong>sure</strong> you want to sign out?");
        var result = await dialog.Result;
        if (result.Cancelled)
            return;

        await AuthClient.SignOutAsync();
    }

   
}
