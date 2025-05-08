namespace Shared.Services;

public class FileDownloadService : IFileDownloadService
{
    private readonly IJSRuntime _jsRuntime;

    public FileDownloadService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task DownloadFileAsync(FileResult file)
    {
        // Call the JS function using JSInterop
        await _jsRuntime.InvokeVoidAsync("downloadFileFromByteArray", file.ContentType, file.Name, file.Contents);
    }
}