using CloudFileServer.Domain.Interfaces;

namespace CloudFileServer.Domain.Models.TreeItems;

public abstract class NodeTreeItem
{
    public long Id { get; }
    public string Name { get; }
    public NodeTypeCode TypeCode { get; }
    public IReadOnlyList<Tag> Tags => _tags.AsReadOnly();
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    public abstract bool IsDirectory { get; }

    public abstract void Accept(INodeVisitor visitor);

    private readonly List<Tag> _tags = new();

    protected NodeTreeItem(
        long id,
        string name,
        NodeTypeCode typeCode,
        IReadOnlyList<Tag> tags,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Id = id;
        Name = name;
        TypeCode = typeCode;
        _tags.AddRange(tags);
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public void AddTag(Tag tag) => _tags.Add(tag);

    public void RemoveTag(Tag tag) => _tags.Remove(tag);
}
