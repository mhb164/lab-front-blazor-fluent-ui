namespace Shared.Interfaces;
public interface IFileDownloadService
{
    Task DownloadFileAsync(FileResult file);
}