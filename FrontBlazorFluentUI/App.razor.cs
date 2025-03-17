namespace Laboratory.Front;

public partial class App
{
    [Inject] public required IDialogService DialogService { get; set; } = default!;
    [Inject] public required AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] public required NavigationManager Navigation { get; set; } = default!;
    [Inject] private UserContext UserContext { get; set; } = default!;

    private bool _disposed = false;
    private IDialogReference? _changePasswordDialog;

    protected override async Task OnInitializedAsync()
    {
        AuthStateProvider.AuthenticationStateChanged += AuthenticationStateChangedHandler;
        AuthenticationStateChangedHandler(AuthStateProvider.GetAuthenticationStateAsync());
        await Task.CompletedTask;
    }

    private async void AuthenticationStateChangedHandler(Task<AuthenticationState> task)
    {
        await task;
        await HandleChangePasswordAsync();
    }

    private Task HandleChangePasswordAsync()
    {
        if (_changePasswordDialog != null && !UserContext.ForceChangeLocalPassword)
            return HideChangePasswordAsync();

        return ShowChangePasswordAsync();
    }

    private async Task ShowChangePasswordAsync()
    {
        if (_changePasswordDialog != null)
            return;

        if (!UserContext.ForceChangeLocalPassword)
            return;

        _changePasswordDialog = await DialogService.ShowDialogAsync<ChangeLocalPasswordPanel>(new DialogParameters()
        {
            Modal = false,
            PreventDismissOnOverlayClick = true,
        });

        var result = await _changePasswordDialog.Result;
        _changePasswordDialog = null;
    }

    private async Task HideChangePasswordAsync()
    {
        if (_changePasswordDialog == null)
            return;

        await _changePasswordDialog.CloseAsync();
        _changePasswordDialog = null;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            AuthStateProvider.AuthenticationStateChanged -= AuthenticationStateChangedHandler;
        }
    }
}
