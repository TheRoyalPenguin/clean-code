namespace WebApp.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(string fileName, string contentType, Stream fileStream);
    Task<Stream> GetFileAsync(string objectName);
    Task DeleteFileAsync(string objectName);
}
