using CloudFileServer.Domain.Models.TreeItems;

namespace CloudFileServer.Domain.Interfaces;

public interface INodeVisitor
{
    void EnterDirectory(DirectoryTreeItem directory);
    void LeaveDirectory(DirectoryTreeItem directory);
    void Visit(ImageFileTreeItem image);
    void Visit(TextFileTreeItem text);
    void Visit(WordFileTreeItem word);
}
