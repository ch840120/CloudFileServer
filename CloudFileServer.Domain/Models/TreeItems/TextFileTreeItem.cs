using CloudFileServer.Domain.Interfaces;

namespace CloudFileServer.Domain.Models.TreeItems;

public class TextFileTreeItem : FileTreeItem
{
    public string Encoding { get; }

    public TextFileTreeItem(
        long id,
        string name,
        long sizeBytes,
        string storagePath,
        string encoding,
        IReadOnlyList<Tag> tags,
        DateTime createdAt,
        DateTime updatedAt)
        : base(id, name, NodeTypeCode.Text, sizeBytes, storagePath, tags, createdAt, updatedAt)
    {
        Encoding = encoding;
    }

    public override void Accept(INodeVisitor visitor) => visitor.Visit(this);
}
