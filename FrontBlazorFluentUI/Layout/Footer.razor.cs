using FrontBlazorFluentUI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System;

namespace FrontBlazorFluentUI.Layout;

public sealed partial class Footer : IDisposable
{
    [Inject] private UserContext UserContext { get; set; } = default!;

    [Inject] private TimeService TimeService { get; set; } = default!;

    private string? currentTime;
    private Color TimeColor { get; set; } = Color.Accent;

    protected override void OnInitialized()
    {
        UpdateUI(true);
        TimeService.OnTimeChanged += OnTimeChanged;
        UserContext.UserChanged += OnUserChanged;
    }

    public void Dispose()
    {
        TimeService.OnTimeChanged -= OnTimeChanged;
        UserContext.UserChanged -= OnUserChanged;
    }

    private void OnTimeChanged(DateTime newTime) => UpdateUI();
    private void OnUserChanged() => UpdateUI();

    private void UpdateUI(bool onInitialized = false)
    {
        currentTime = $"{TimeService.CurrentTime:yyyy-MM-dd HH:mm:ss}";

        if (!UserContext.ServerAccessible)
        {
            TimeColor = Color.Error;
        }
        else if (!UserContext.Authenticated)
        {
            TimeColor = Color.Warning;
        }
        else
        {
            TimeColor = Color.Accent;
        }

        if (onInitialized)
            return;
        InvokeAsync(StateHasChanged);
    }

}
