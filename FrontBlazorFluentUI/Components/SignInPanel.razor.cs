namespace Laboratory.Front.Components;

public partial class SignInPanel
{
    [Inject] public required AuthClient AuthService { get; set; }

    [CascadingParameter] public FluentDialog Dialog { get; set; } = default!;

    private SignInModel _content = new();
    private EditContext _editContext = default!;

    public bool ErrorMessageHasValue => _errorMessage != null;
    private string? _errorMessage;
    private bool _isSubmiting = false;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_content);
    }

    private string StyleForSubmit()
    {
        if (_editContext.Validate()) return "color: var(--accent-base-color);";
        return "color: var(--warning);";
    }

    private async Task SubmitSignInAsync()
    {
        if (!_editContext.Validate())
            return;

        _isSubmiting = true;

        var result = await AuthService.SignInAsync(_content.ToApiModel());
        if (result.IsSuccess)
        {
            await Dialog.CloseAsync();
            return;
        }

        _errorMessage = result.Message;
        _isSubmiting = false;
    }

    private async Task OnCloseAsync()
    {
        await Dialog.CloseAsync();
    }

    private sealed class SignInModel
    {
        [MinLength(1, ErrorMessage = $"{nameof(Username)} Should not be empty")]
        public string? Username { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = $"{nameof(Password)} Should not be empty")]
        public string? Password { get; set; } = string.Empty;

        public SignInRequest ToApiModel()
            => new SignInRequest(Username, Password);
    }
}
