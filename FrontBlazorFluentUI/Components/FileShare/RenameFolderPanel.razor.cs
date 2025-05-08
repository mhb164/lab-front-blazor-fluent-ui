using Laboratory.Frontend.Services;
using Shared.FileShare;

namespace Laboratory.Front.Components.FileShare;

public partial class RenameFolderPanel : IDialogContentComponent<string>
{
    [Inject] public required FileShareClient FileShareService { get; set; }
    [CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
    [Inject] private UserContext UserContext { get; set; } = default!;

    [Parameter] public string Content { get; set; } = default!;

    private RenameFolderModel _editContent = default!;
    private EditContext _editContext = default!;

    public bool ErrorMessageHasValue => _errorMessage != null;
    private string? _errorMessage;
    private bool _isSubmiting = false;

    protected override async Task OnInitializedAsync()
    {
        _editContent = new RenameFolderModel() { NewName = Content };
        _editContext = new EditContext(_editContent);
    }

    private string StyleForSubmit()
    {
        if (_editContext.Validate()) return "color: var(--accent-base-color);";
        return "color: var(--warning);";
    }

    private async Task SubmitAsync()
    {
        if (!_editContext.Validate())
            return;

        _isSubmiting = true;

        var result = default(ServiceResult<FolderList>);
        var request = _editContent.ToApiAddModel(Content);
        result = await FileShareService.HandleAsync(request);

        if (result.IsSuccess)
        {
            await Dialog.CloseAsync(new Tuple<string, FolderList>(request.NewName!, result.Value!));
            return;
        }

        _errorMessage = result.Message;
        _isSubmiting = false;
    }

    private async Task OnCloseAsync()
    {
        await Dialog.CancelAsync();
    }

    private sealed class RenameFolderModel
    {
        [MinLength(1, ErrorMessage = $"{nameof(NewName)} Should not be empty")]
        public string NewName { get; set; }

        public RenameFolderRequest ToApiAddModel(string name)
            => new RenameFolderRequest(name, NewName);

    }
}
