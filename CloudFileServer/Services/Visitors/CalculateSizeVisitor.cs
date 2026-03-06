using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models.TreeItems;

namespace CloudFileServer.Services.Visitors;

public class CalculateSizeVisitor : INodeVisitor
{
    public long TotalBytes { get; private set; }

    public void EnterDirectory(DirectoryTreeItem directory) { }
    public void LeaveDirectory(DirectoryTreeItem directory) { }
    public void Visit(ImageFileTreeItem image) => TotalBytes += image.SizeBytes;
    public void Visit(TextFileTreeItem text)   => TotalBytes += text.SizeBytes;
    public void Visit(WordFileTreeItem word)   => TotalBytes += word.SizeBytes;
}
