using CloudFileServer.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CloudFileServer.Persistent.Services;

public class MockFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public MockFileStorageService(IConfiguration configuration)
    {
        _basePath = configuration["AppSettings:FileStoragePath"] ?? string.Empty;
    }

    public void CopyFile(string sourcePath, string destinationPath)
    {
        var fullSource = ResolvePath(sourcePath);
        var fullDest = ResolvePath(destinationPath);

        if (!File.Exists(fullSource))
            return; // Source file absent (seed data scenario); skip silently

        var destDir = Path.GetDirectoryName(fullDest);
        if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        File.Copy(fullSource, fullDest, overwrite: false);
    }

    public void DeleteFile(string filePath)
    {
        var fullPath = ResolvePath(filePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    private string ResolvePath(string relativePath)
    {
        var normalized = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        return string.IsNullOrEmpty(_basePath)
            ? normalized
            : Path.Combine(_basePath, normalized);
    }
}
