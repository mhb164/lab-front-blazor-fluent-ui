using Laboratory.Frontend.Services;
using Shared.FileShare;

namespace Laboratory.Front.Components.FileShare;

public partial class AddNewFolderPanel : IDialogContentComponent
{
    [Inject] public required FileShareClient FileShareService { get; set; }
    [CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
    [Inject] private UserContext UserContext { get; set; } = default!;

    private CreateFolderModel _editContent = default!;
    private EditContext _editContext = default!;

    public bool ErrorMessageHasValue => _errorMessage != null;
    private string? _errorMessage;
    private bool _isSubmiting = false;

    protected override async Task OnInitializedAsync()
    {
        _editContent = new CreateFolderModel() { Name = $"{UserContext?.Title}-{DateTime.Now:yyyy_MM_dd-HH_mm}" };
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
        var request = _editContent.ToApiAddModel();
        result = await FileShareService.HandleAsync(request);

        if (result.IsSuccess)
        {
            await Dialog.CloseAsync(new Tuple<string, FolderList>(request.Name!, result.Value!));
            return;
        }

        _errorMessage = result.Message;
        _isSubmiting = false;
    }

    private async Task OnCloseAsync()
    {
        await Dialog.CancelAsync();
    }

    private sealed class CreateFolderModel
    {
        [MinLength(1, ErrorMessage = $"{nameof(Name)} Should not be empty")]
        public string Name { get; set; }

        public CreateFolderRequest ToApiAddModel()
            => new CreateFolderRequest(Name);

    }
}
