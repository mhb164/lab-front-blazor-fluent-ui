using Laboratory.Frontend.Services;
using Shared.FileShare;

namespace Laboratory.Front.Pages;

public partial class FileShare
{
    [Inject] public required IDialogService DialogService { get; set; } = default!;
    [Inject] public required IFileDownloadService FileDownloadService { get; set; }
    [Inject] public required FileShareClient FileShareService { get; set; }
    [Inject] public required LayoutService LayoutService { get; set; }
    [Inject] private UserContext UserContext { get; set; } = default!;

    private FolderList? folders;
    private string? _selectedFolder;
    public string? SelectedFolder
    {
        get => _selectedFolder;
        set
        {
            _selectedFolder = value;
            InvokeAsync(OnSelectedFolderChanged);
        }
    }
    private FolderFileList? folderFileList;

    protected override async Task OnInitializedAsync()
    {
        LayoutService.Title = "File Share";

        await RefreshFoldersAsync();
    }

    private async Task RefreshFoldersAsync()
    {
        var preSelectedFolder = SelectedFolder;
        var getResult = await FileShareService.GetFoldersAsync();

        await Refresh(getResult.Value, preSelectedFolder);
    }

    private async Task Refresh(FolderList? value, string? preSelectedFolder)
    {
        folders = value;
        StateHasChanged();
        SelectedFolder = preSelectedFolder ?? folders?.Items?.FirstOrDefault();
    }

    private async Task OnGetFoldersClick() => await RefreshFoldersAsync();

    private async Task OnFolderAddClick()
    {
        var dialog = await DialogService.ShowDialogAsync<Components.FileShare.AddNewFolderPanel>(new DialogParameters());
        var result = await dialog.Result;

        if (result is null || result.Cancelled)
            return;

        var resultData = result.Data as Tuple<string, FolderList>;
        await Refresh(resultData?.Item2, resultData?.Item1);
    }

    private async Task OnFolderRenameClick()
    {
        if (SelectedFolder is null)
            return;

        var dialog = await DialogService.ShowDialogAsync<Components.FileShare.RenameFolderPanel>(SelectedFolder, new DialogParameters());
        var result = await dialog.Result;

        if (result is null || result.Cancelled)
            return;

        var resultData = result.Data as Tuple<string, FolderList>;
        await Refresh(resultData?.Item2, resultData?.Item1);
    }

    private async Task OnFolderDeleteClick()
    {
        if (SelectedFolder is null)
            return;

        var dialog = await DialogService.ShowConfirmationAsync($"Are you <strong>sure</strong> you want to delete '{SelectedFolder}'?");
        var result = await dialog.Result;
        if (result.Cancelled)
            return;

        var request = new DeleteFolderRequest(SelectedFolder);
        var handleResult = await FileShareService.HandleAsync(request);

        if (handleResult.IsSuccess)
        {
            await Refresh(handleResult.Value, null);
            return;
        }

        await DialogService.ShowErrorAsync(handleResult.Message!);
        await dialog.Result;

        await RefreshFoldersAsync();
    }

    private async Task OnSelectedFolderChanged()
    {
        if (SelectedFolder != null)
        {
            var handleResult = await FileShareService.HandleAsync(new FolderFilesRequest(SelectedFolder));
            folderFileList = handleResult?.Value;
        }
        else
        {
            folderFileList = null;
        }
        StateHasChanged();
    }


    FluentInputFile? myFileByStream = default!;
    int? progressPercent;
    string? progressTitle;
    List<string> Files = new();

    async Task OnFileUploadedAsync(FluentInputFileEventArgs file)
    {
        if (SelectedFolder is null)
            return;

        progressPercent = file.ProgressPercent;
        progressTitle = file.ProgressTitle;

        await FileShareService.UploadAsync(SelectedFolder, file.Stream, file.Name);
        await file.Stream.DisposeAsync();
    }

    void OnCompleted(IEnumerable<FluentInputFileEventArgs> files)
    {
        progressPercent = myFileByStream!.ProgressPercent;
        progressTitle = myFileByStream!.ProgressTitle;

        // For the demo, delete these files.
        foreach (var file in Files)
        {
            File.Delete(file);
        }

        InvokeAsync(RefreshFoldersAsync);
    }

    private async Task DownloadAsync(string folder, string name)
    {
        try
        {
            var result = await FileShareService.HandleAsync(new DownloadFileRequest(folder, name));
            if (result.IsSuccess)
            {
                await FileDownloadService.DownloadFileAsync(result.Value!);
            }
            else
            {
            }
        }
        catch (Exception ex)
        {
        }
    }

    private async Task DeleteFileAsync(string folder, string name)
    {
        var dialog = await DialogService.ShowConfirmationAsync($"Are you <strong>sure</strong> you want to delete '{name}' from '{folder}'?");
        var result = await dialog.Result;
        if (result.Cancelled)
            return;

        var request = new DeleteFileRequest(folder, name);
        var handleResult = await FileShareService.HandleAsync(request);

        if (handleResult.IsSuccess)
        {
            folderFileList = handleResult.Value;
            StateHasChanged();
            return;
        }

        await DialogService.ShowErrorAsync(handleResult.Message!);
        await dialog.Result;
    }
}
