using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models.TreeItems;

namespace CloudFileServer.Services.Visitors;

public class TraversalLogDecorator : INodeVisitor
{
    private readonly INodeVisitor _inner;
    private readonly List<string> _pathStack = new();
    private readonly List<string> _log = new();

    public IReadOnlyList<string> Log => _log;

    public TraversalLogDecorator(INodeVisitor inner, IEnumerable<string>? ancestorPath = null)
    {
        _inner = inner;
        if (ancestorPath is not null)
            _pathStack.AddRange(ancestorPath);
    }

    public void EnterDirectory(DirectoryTreeItem directory)
    {
        _pathStack.Add(directory.Name);
        _log.Add($"Visiting: {string.Join(" -> ", _pathStack)}");
        _inner.EnterDirectory(directory);
    }

    public void LeaveDirectory(DirectoryTreeItem directory)
    {
        _inner.LeaveDirectory(directory);
        _pathStack.RemoveAt(_pathStack.Count - 1);
    }

    public void Visit(ImageFileTreeItem image)
    {
        _log.Add($"Visiting: {BuildPath(image.Name, image.StoragePath)}");
        _inner.Visit(image);
    }

    public void Visit(TextFileTreeItem text)
    {
        _log.Add($"Visiting: {BuildPath(text.Name, text.StoragePath)}");
        _inner.Visit(text);
    }

    public void Visit(WordFileTreeItem word)
    {
        _log.Add($"Visiting: {BuildPath(word.Name, word.StoragePath)}");
        _inner.Visit(word);
    }

    private string BuildPath(string name, string storagePath)
    {
        var ext = storagePath.Contains('.')
            ? storagePath.Substring(storagePath.LastIndexOf('.'))
            : string.Empty;
        return string.Join(" -> ", _pathStack.Append(name + ext));
    }
}
