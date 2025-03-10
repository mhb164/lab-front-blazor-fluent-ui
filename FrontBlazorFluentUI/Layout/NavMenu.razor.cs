using FrontBlazorFluentUI.Services;
using Microsoft.AspNetCore.Components;
using System;

namespace FrontBlazorFluentUI.Layout;

public sealed partial class NavMenu : IDisposable
{
    [Inject] private UserContext UserContext { get; set; } = default!;

    private bool _serverAccessible = false;
    private string? _navMenuTitle = "Navigation";

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

    private void OnNavMenuClick()
    {
    } 
}
