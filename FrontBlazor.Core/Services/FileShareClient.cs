

namespace Laboratory.Frontend.Services;

public class FileShareClient
{
    private readonly ILogger? _logger;
    private readonly IApiClient _apiClient;

    public FileShareClient(ILogger<FileShareClient>? logger, IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public async Task<ServiceResult<FolderList>> GetFoldersAsync()
    {
        var serviceResult = await _apiClient.Post<BlankRequest, FolderList>("/share/folders", BlankRequest.Instance);
        return serviceResult;
    }

    public async Task<ServiceResult<FolderList>> HandleAsync(CreateFolderRequest request)
    {
        var serviceResult = await _apiClient.Post<CreateFolderRequest, FolderList>("/share/create-folder", request);
        return serviceResult;
    }

    public async Task<ServiceResult<FolderList>> HandleAsync(RenameFolderRequest request)
    {
        var serviceResult = await _apiClient.Post<RenameFolderRequest, FolderList>("/share/rename-folder", request);
        return serviceResult;
    }

    public async Task<ServiceResult<FolderList>> HandleAsync(DeleteFolderRequest request)
    {
        var serviceResult = await _apiClient.Post<DeleteFolderRequest, FolderList>("/share/delete-folder", request);
        return serviceResult;
    }

    public async Task<ServiceResult<FolderFileList>> HandleAsync(FolderFilesRequest request)
    {
        var serviceResult = await _apiClient.Post<FolderFilesRequest, FolderFileList>("/share/files", request);
        return serviceResult;
    }

    public async Task UploadAsync(string folder, Stream stream, string fileName)
    {
        var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        var form = new MultipartFormDataContent
        {
            { streamContent, "files", fileName }
        };
        await _apiClient.PostContent($"/share/upload/{folder}", form);
    }

    public async Task<ServiceResult<FileResult>> HandleAsync(DownloadFileRequest request)
        => await _apiClient.PostDownload($"/share/download", request);

    public async Task<ServiceResult<FolderFileList>> HandleAsync(DeleteFileRequest request)
        => await _apiClient.Post<DeleteFileRequest, FolderFileList>("/share/delete-file", request);
}
