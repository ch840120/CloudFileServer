using CloudFileServer.Domain.Models;
using CloudFileServer.Domain.Models.TreeItems;
using CloudFileServer.Services.Visitors;

namespace CloudFileServer.Tests.Services.Visitors;

public class CalculateSizeVisitorTests
{
    private static readonly DateTime Now = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static TextFileTreeItem  MakeText (long id, long size) =>
        new(id, $"f{id}", size, $"/f{id}.txt", "UTF-8", Array.Empty<Tag>(), Now, Now);

    private static ImageFileTreeItem MakeImage(long id, long size) =>
        new(id, $"i{id}", size, $"/i{id}.jpg", 100, 100, Array.Empty<Tag>(), Now, Now);

    private static WordFileTreeItem  MakeWord (long id, long size) =>
        new(id, $"w{id}", size, $"/w{id}.docx", 5, Array.Empty<Tag>(), Now, Now);

    private static DirectoryTreeItem MakeDir(long id, params NodeTreeItem[] children)
    {
        var dir = new DirectoryTreeItem(id, $"dir{id}", Array.Empty<Tag>(), Now, Now);
        foreach (var child in children) dir.AddChild(child);
        return dir;
    }

    [Fact]
    public void Visit_TextFile_AccumulatesSize()
    {
        var visitor = new CalculateSizeVisitor();
        MakeText(1, 512).Accept(visitor);
        Assert.Equal(512, visitor.TotalBytes);
    }

    [Fact]
    public void Visit_ImageFile_AccumulatesSize()
    {
        var visitor = new CalculateSizeVisitor();
        MakeImage(1, 2048).Accept(visitor);
        Assert.Equal(2048, visitor.TotalBytes);
    }

    [Fact]
    public void Visit_WordFile_AccumulatesSize()
    {
        var visitor = new CalculateSizeVisitor();
        MakeWord(1, 4096).Accept(visitor);
        Assert.Equal(4096, visitor.TotalBytes);
    }

    [Fact]
    public void Visit_MultipleFiles_SumsAllSizes()
    {
        var visitor = new CalculateSizeVisitor();
        MakeText (1, 100).Accept(visitor);
        MakeImage(2, 200).Accept(visitor);
        MakeWord (3, 300).Accept(visitor);
        Assert.Equal(600, visitor.TotalBytes);
    }

    [Fact]
    public void Visit_DirectoryWithChildren_SumsDescendants()
    {
        var visitor = new CalculateSizeVisitor();
        MakeDir(1, MakeText(2, 100), MakeImage(3, 200)).Accept(visitor);
        Assert.Equal(300, visitor.TotalBytes);
    }

    [Fact]
    public void Visit_NestedDirectories_SumsAllLevels()
    {
        var visitor = new CalculateSizeVisitor();
        var tree = MakeDir(1,
            MakeDir(2,
                MakeText(3, 100),
                MakeText(4, 200)),
            MakeImage(5, 400));
        tree.Accept(visitor);
        Assert.Equal(700, visitor.TotalBytes);
    }

    [Fact]
    public void Visit_EmptyDirectory_ReturnsZero()
    {
        var visitor = new CalculateSizeVisitor();
        MakeDir(1).Accept(visitor);
        Assert.Equal(0, visitor.TotalBytes);
    }

    [Fact]
    public void Visit_DirectoryNotCountedItself_OnlyFilesContribute()
    {
        // Directory has no SizeBytes — only its file children contribute
        var visitor = new CalculateSizeVisitor();
        MakeDir(1, MakeText(2, 50)).Accept(visitor);
        Assert.Equal(50, visitor.TotalBytes);
    }
}
