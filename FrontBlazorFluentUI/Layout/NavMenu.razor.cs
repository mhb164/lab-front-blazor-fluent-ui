using FrontBlazorFluentUI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System;

namespace FrontBlazorFluentUI.Layout;

public sealed partial class NavMenu : IDisposable
{
    [Inject] private UserContext UserContext { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;

    private bool _serverAccessible = false;
    private string? _navMenuTitle = "Navigation";
    private IDialogReference? _navigationDialog;

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
        _navigationDialog = await DialogService.ShowPanelAsync<NavigationPanel>(new DialogParameters()
        {
            ShowTitle = true,
            Title = "Fluent UI",
            Alignment = HorizontalAlignment.Left,
            PrimaryAction = null,
            SecondaryAction = null,
            ShowDismiss = true
        });

        DialogResult result = await _navigationDialog.Result;
    } 
}
