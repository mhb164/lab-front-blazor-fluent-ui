﻿namespace Laboratory.Front.Layout;

public sealed partial class NavMenu : IDisposable
{
    [Inject] private UserContext UserContext { get; set; } = default!;

    private bool _serverAccessible = false;
    private string? _navMenuTitle = "Front LAB";

    bool ShowNavMenu = false;

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

        if (onInitialized)
            return;
        InvokeAsync(StateHasChanged);
    }

    private async Task OnNavMenuClickAsync()
    {
        //var options = new DialogOptions
        //{
        //    CssClass = "custom-panel" // Apply the CSS override
        //};
        ShowNavMenu = true;
        //_navigationDialog = await DialogService.ShowPanelAsync<NavigationPanel>(new DialogParameters()
        //{
        //    Alignment = HorizontalAlignment.Left,
        //    PrimaryAction = null,
        //    SecondaryAction = null,
        //    ShowDismiss = true,
        //    DialogType = DialogType.Panel,
        //});

        //DialogResult result = await _navigationDialog.Result;
    }

    private async Task OnCloseAsync()
    {
        ShowNavMenu = false;
    }

}
