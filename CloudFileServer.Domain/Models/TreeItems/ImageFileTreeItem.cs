namespace CloudFileServer.Domain.Models.TreeItems;

public class ImageFileTreeItem : FileTreeItem
{
    public int WidthPx { get; }
    public int HeightPx { get; }

    public ImageFileTreeItem(
        long id,
        string name,
        long sizeBytes,
        string storagePath,
        int widthPx,
        int heightPx,
        IReadOnlyList<Tag> tags,
        DateTime createdAt,
        DateTime updatedAt)
        : base(id, name, NodeTypeCode.Image, sizeBytes, storagePath, tags, createdAt, updatedAt)
    {
        WidthPx = widthPx;
        HeightPx = heightPx;
    }
}
