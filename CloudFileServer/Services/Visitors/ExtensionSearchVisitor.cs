using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models.TreeItems;

namespace CloudFileServer.Services.Visitors;

public class ExtensionSearchVisitor : INodeVisitor
{
    private readonly string _extension;

    public List<NodeTreeItem> Results { get; } = new();

    public ExtensionSearchVisitor(string extension)
    {
        _extension = extension.StartsWith('.') ? extension : "." + extension;
    }

    public void EnterDirectory(DirectoryTreeItem directory) { }
    public void LeaveDirectory(DirectoryTreeItem directory) { }

    public void Visit(ImageFileTreeItem image)
    {
        if (MatchesExtension(image.StoragePath)) Results.Add(image);
    }

    public void Visit(TextFileTreeItem text)
    {
        if (MatchesExtension(text.StoragePath)) Results.Add(text);
    }

    public void Visit(WordFileTreeItem word)
    {
        if (MatchesExtension(word.StoragePath)) Results.Add(word);
    }

    private bool MatchesExtension(string storagePath)
    {
        var dotIndex = storagePath.LastIndexOf('.');
        if (dotIndex < 0) return false;
        var ext = storagePath.Substring(dotIndex);
        return ext.Equals(_extension, StringComparison.OrdinalIgnoreCase);
    }
}
