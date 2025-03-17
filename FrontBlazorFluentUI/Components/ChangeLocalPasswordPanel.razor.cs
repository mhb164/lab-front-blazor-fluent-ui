namespace Laboratory.Front.Components;

public partial class ChangeLocalPasswordPanel
{
    [Inject] public required UserContext UserContext { get; set; }
    [Inject] public required AuthClient AuthService { get; set; }
    [CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

    private ChangePasswordModel _content = new();
    private EditContext _editContext = default!;

    public bool ErrorMessageHasValue => _errorMessage != null;
    private string? _errorMessage;
    private bool _isSubmiting = false;

    private bool _forceChangeLocalPassword = true;

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_content);
        _forceChangeLocalPassword = UserContext.ForceChangeLocalPassword;

        await Task.CompletedTask;
    }

    private string StyleForSubmit()
    {
        if (_editContext.Validate()) return "color: var(--accent-base-color);";
        return "color: var(--warning);";
    }

    private async Task SubmitChangePasswordAsync()
    {
        if (!_editContext.Validate())
            return;

        if (_content.NewPassword != _content.ConfirmPassword)
        {
            _errorMessage = "Passwords do not match!";
            return;
        }

        var result = await AuthService.ChangePasswordAsync(_content.ToApiModel());

        if (result.IsSuccess)
        {
            await Dialog.CloseAsync();
            return;
        }


        _errorMessage = result.Message;
        _isSubmiting = false;
    }

    private async Task HandleSignOutAsync()
    {
        await AuthService.SignOutAsync();
        await Dialog.CloseAsync();
    }
    private async Task OnCloseAsync()
    {
        await Dialog.CloseAsync();
    }

    private sealed class ChangePasswordModel
    {
        [Required(ErrorMessage = "Current password is required")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(1, ErrorMessage = "Password must be at least 1 characters long")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirm Password do not match")]
        public string? ConfirmPassword { get; set; }

        public ChangeLocalPasswordRequest ToApiModel()
           => new ChangeLocalPasswordRequest(CurrentPassword, NewPassword);
    }

}
