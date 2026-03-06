using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models;
using CloudFileServer.Domain.Models.TreeItems;
using CloudFileServer.Services.Visitors;
using Moq;

namespace CloudFileServer.Tests.Services.Visitors;

public class TraversalLogDecoratorTests
{
    private static readonly DateTime Now = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static TextFileTreeItem  MakeText (long id, string name, string path) =>
        new(id, name, 100, path, "UTF-8", Array.Empty<Tag>(), Now, Now);

    private static ImageFileTreeItem MakeImage(long id, string name, string path) =>
        new(id, name, 200, path, 100, 100, Array.Empty<Tag>(), Now, Now);

    private static DirectoryTreeItem MakeDir(long id, string name, params NodeTreeItem[] children)
    {
        var dir = new DirectoryTreeItem(id, name, Array.Empty<Tag>(), Now, Now);
        foreach (var child in children) dir.AddChild(child);
        return dir;
    }

    // ── Log content ──────────────────────────────────────────────────────

    [Fact]
    public void Log_SingleDirectory_RecordsDirVisit()
    {
        var decorator = new TraversalLogDecorator(new CalculateSizeVisitor());
        MakeDir(1, "docs").Accept(decorator);
        Assert.Single(decorator.Log);
        Assert.Contains("docs", decorator.Log[0]);
    }

    [Fact]
    public void Log_NestedDirectories_RecordsChainedPath()
    {
        var decorator = new TraversalLogDecorator(new CalculateSizeVisitor());
        MakeDir(1, "root",
            MakeDir(2, "sub")).Accept(decorator);

        var chainEntry = decorator.Log.FirstOrDefault(l => l.Contains("root") && l.Contains("sub"));
        Assert.NotNull(chainEntry);
        Assert.Contains("->", chainEntry);
    }

    [Fact]
    public void Log_TextFile_RecordsNameWithExtension()
    {
        var decorator = new TraversalLogDecorator(new CalculateSizeVisitor());
        MakeText(1, "readme", "/readme.txt").Accept(decorator);
        Assert.Single(decorator.Log);
        Assert.Contains("readme.txt", decorator.Log[0]);
    }

    [Fact]
    public void Log_ImageFile_RecordsNameWithExtension()
    {
        var decorator = new TraversalLogDecorator(new CalculateSizeVisitor());
        MakeImage(1, "photo", "/photo.jpg").Accept(decorator);
        Assert.Single(decorator.Log);
        Assert.Contains("photo.jpg", decorator.Log[0]);
    }

    [Fact]
    public void Log_FileInsideDirectory_RecordsFullPath()
    {
        var decorator = new TraversalLogDecorator(new CalculateSizeVisitor());
        MakeDir(1, "docs", MakeText(2, "readme", "/docs/readme.txt")).Accept(decorator);

        var fileEntry = decorator.Log.FirstOrDefault(l => l.Contains("readme.txt"));
        Assert.NotNull(fileEntry);
        Assert.Contains("docs", fileEntry);
        Assert.Contains("->", fileEntry);
    }

    [Fact]
    public void Log_ComplexTree_RecordsEntriesForEveryNode()
    {
        var decorator = new TraversalLogDecorator(new CalculateSizeVisitor());
        MakeDir(1, "root",
            MakeDir(2, "sub",
                MakeText(3, "a", "/a.txt"),
                MakeText(4, "b", "/b.txt")),
            MakeImage(5, "photo", "/photo.jpg")).Accept(decorator);

        // root dir + sub dir + a.txt + b.txt + photo.jpg = 5 log entries
        Assert.Equal(5, decorator.Log.Count);
    }

    // ── Delegation to inner visitor ──────────────────────────────────────

    [Fact]
    public void Decorator_DelegatesEnterDirectory_ToInner()
    {
        var innerMock = new Mock<INodeVisitor>();
        var decorator = new TraversalLogDecorator(innerMock.Object);
        var dir = MakeDir(1, "docs");

        dir.Accept(decorator);

        innerMock.Verify(v => v.EnterDirectory(dir), Times.Once);
        innerMock.Verify(v => v.LeaveDirectory(dir), Times.Once);
    }

    [Fact]
    public void Decorator_DelegatesVisitTextFile_ToInner()
    {
        var innerMock = new Mock<INodeVisitor>();
        var decorator = new TraversalLogDecorator(innerMock.Object);
        var file = MakeText(1, "readme", "/readme.txt");

        file.Accept(decorator);

        innerMock.Verify(v => v.Visit(file), Times.Once);
    }

    [Fact]
    public void Decorator_DelegatesVisitImageFile_ToInner()
    {
        var innerMock = new Mock<INodeVisitor>();
        var decorator = new TraversalLogDecorator(innerMock.Object);
        var img = MakeImage(1, "photo", "/photo.jpg");

        img.Accept(decorator);

        innerMock.Verify(v => v.Visit(img), Times.Once);
    }

    [Fact]
    public void Decorator_PathStack_ClearedAfterLeavingDirectory()
    {
        // After traversing a directory, path stack should be empty
        // so a sibling directory does not carry over the previous path
        var decorator = new TraversalLogDecorator(new CalculateSizeVisitor());
        var root = new DirectoryTreeItem(1, "root", Array.Empty<Tag>(), Now, Now);
        root.AddChild(MakeDir(2, "alpha"));
        root.AddChild(MakeDir(3, "beta"));
        root.Accept(decorator);

        var betaEntry = decorator.Log.First(l => l.Contains("beta"));
        // "beta" entry should NOT contain "alpha" (stack was cleared)
        Assert.DoesNotContain("alpha", betaEntry);
    }
}
