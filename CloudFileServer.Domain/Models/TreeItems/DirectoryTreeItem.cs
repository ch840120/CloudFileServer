using CloudFileServer.Domain.Interfaces;

namespace CloudFileServer.Domain.Models.TreeItems;

public class DirectoryTreeItem : NodeTreeItem
{
    private readonly List<NodeTreeItem> _children = new();

    public IReadOnlyList<NodeTreeItem> Children => _children.AsReadOnly();

    public override bool IsDirectory => true;

    public DirectoryTreeItem(
        long id,
        string name,
        IReadOnlyList<Tag> tags,
        DateTime createdAt,
        DateTime updatedAt)
        : base(id, name, NodeTypeCode.Directory, tags, createdAt, updatedAt)
    {
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.EnterDirectory(this);
        foreach (var child in _children)
            child.Accept(visitor);
        visitor.LeaveDirectory(this);
    }

    public void AddChild(NodeTreeItem child)
    {
        _children.Add(child);
    }
}
