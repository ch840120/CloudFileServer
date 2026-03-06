namespace CloudFileServer.Domain.Interfaces;

public interface IFileStorageService
{
    void CopyFile(string sourcePath, string destinationPath);
    void DeleteFile(string filePath);
}
