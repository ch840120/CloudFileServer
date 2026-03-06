using CloudFileServer.Domain.Interfaces;

namespace CloudFileServer.Domain.Models.TreeItems;

public class WordFileTreeItem : FileTreeItem
{
    public int PageCount { get; }

    public WordFileTreeItem(
        long id,
        string name,
        long sizeBytes,
        string storagePath,
        int pageCount,
        IReadOnlyList<Tag> tags,
        DateTime createdAt,
        DateTime updatedAt)
        : base(id, name, NodeTypeCode.Word, sizeBytes, storagePath, tags, createdAt, updatedAt)
    {
        PageCount = pageCount;
    }

    public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
}
