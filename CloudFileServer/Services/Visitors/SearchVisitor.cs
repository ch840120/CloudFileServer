using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models.TreeItems;

namespace CloudFileServer.Services.Visitors;

public class SearchVisitor : INodeVisitor
{
    private readonly string _keyword;

    public List<NodeTreeItem> Results { get; } = new();

    public SearchVisitor(string keyword)
    {
        _keyword = keyword;
    }

    public void EnterDirectory(DirectoryTreeItem directory)
    {
        if (directory.Name.Contains(_keyword, StringComparison.OrdinalIgnoreCase))
            Results.Add(directory);
    }

    public void LeaveDirectory(DirectoryTreeItem directory) { }

    public void Visit(ImageFileTreeItem image)
    {
        if (image.Name.Contains(_keyword, StringComparison.OrdinalIgnoreCase))
            Results.Add(image);
    }

    public void Visit(TextFileTreeItem text)
    {
        if (text.Name.Contains(_keyword, StringComparison.OrdinalIgnoreCase))
            Results.Add(text);
    }

    public void Visit(WordFileTreeItem word)
    {
        if (word.Name.Contains(_keyword, StringComparison.OrdinalIgnoreCase))
            Results.Add(word);
    }
}
