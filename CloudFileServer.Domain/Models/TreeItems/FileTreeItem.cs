namespace CloudFileServer.Domain.Models.TreeItems;

public abstract class FileTreeItem : NodeTreeItem
{
    public long SizeBytes { get; }
    public string StoragePath { get; }

    public override bool IsDirectory => false;

    protected FileTreeItem(
        long id,
        string name,
        NodeTypeCode typeCode,
        long sizeBytes,
        string storagePath,
        IReadOnlyList<Tag> tags,
        DateTime createdAt,
        DateTime updatedAt)
        : base(id, name, typeCode, tags, createdAt, updatedAt)
    {
        SizeBytes = sizeBytes;
        StoragePath = storagePath;
    }
}
