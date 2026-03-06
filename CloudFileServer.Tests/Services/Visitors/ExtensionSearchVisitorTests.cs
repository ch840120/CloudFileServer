using CloudFileServer.Domain.Models;
using CloudFileServer.Domain.Models.TreeItems;
using CloudFileServer.Services.Visitors;

namespace CloudFileServer.Tests.Services.Visitors;

public class ExtensionSearchVisitorTests
{
    private static readonly DateTime Now = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static TextFileTreeItem MakeText(long id, string path) =>
        new(id, $"f{id}", 100, path, "UTF-8", Array.Empty<Tag>(), Now, Now);

    private static WordFileTreeItem MakeWord(long id, string path) =>
        new(id, $"w{id}", 200, path, 5, Array.Empty<Tag>(), Now, Now);

    private static ImageFileTreeItem MakeImage(long id, string path) =>
        new(id, $"i{id}", 300, path, 100, 100, Array.Empty<Tag>(), Now, Now);

    private static DirectoryTreeItem MakeDir(long id, params NodeTreeItem[] children)
    {
        var dir = new DirectoryTreeItem(id, $"dir{id}", Array.Empty<Tag>(), Now, Now);
        foreach (var child in children) dir.AddChild(child);
        return dir;
    }

    [Fact]
    public void Search_WithDotPrefix_MatchesTxtFiles()
    {
        var visitor = new ExtensionSearchVisitor(".txt");
        MakeText(1, "/a.txt").Accept(visitor);
        MakeWord(2, "/b.docx").Accept(visitor);
        Assert.Single(visitor.Results);
        Assert.Equal("/a.txt", ((TextFileTreeItem)visitor.Results[0]).StoragePath);
    }

    [Fact]
    public void Search_WithoutDotPrefix_NormalisesAndMatches()
    {
        // "txt" → normalised to ".txt"
        var visitor = new ExtensionSearchVisitor("txt");
        MakeText(1, "/readme.txt").Accept(visitor);
        Assert.Single(visitor.Results);
    }

    [Fact]
    public void Search_CaseInsensitive_MatchesUpperExt()
    {
        var visitor = new ExtensionSearchVisitor(".TXT");
        MakeText(1, "/readme.txt").Accept(visitor);
        Assert.Single(visitor.Results);
    }

    [Fact]
    public void Search_NoMatch_ReturnsEmpty()
    {
        var visitor = new ExtensionSearchVisitor(".pdf");
        MakeText(1, "/readme.txt").Accept(visitor);
        MakeWord(2, "/report.docx").Accept(visitor);
        Assert.Empty(visitor.Results);
    }

    [Fact]
    public void Search_DirectoryNeverReturned()
    {
        var visitor = new ExtensionSearchVisitor(".txt");
        MakeDir(1, MakeText(2, "/inner.txt")).Accept(visitor);
        Assert.Single(visitor.Results);
        Assert.False(visitor.Results[0].IsDirectory);
    }

    [Fact]
    public void Search_FileWithoutExtension_NotMatched()
    {
        var visitor = new ExtensionSearchVisitor(".txt");
        MakeText(1, "/noext").Accept(visitor); // no dot in path
        Assert.Empty(visitor.Results);
    }

    [Fact]
    public void Search_MultipleMatchingFiles_ReturnsAll()
    {
        var visitor = new ExtensionSearchVisitor(".txt");
        MakeDir(1,
            MakeText(2, "/a.txt"),
            MakeText(3, "/b.txt"),
            MakeWord(4, "/c.docx")).Accept(visitor);
        Assert.Equal(2, visitor.Results.Count);
    }

    [Fact]
    public void Search_ImageByExtension_ReturnsMatch()
    {
        var visitor = new ExtensionSearchVisitor(".jpg");
        MakeImage(1, "/photo.jpg").Accept(visitor);
        Assert.Single(visitor.Results);
        Assert.IsType<ImageFileTreeItem>(visitor.Results[0]);
    }
}
