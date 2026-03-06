using CloudFileServer.Domain.Models;
using CloudFileServer.Domain.Models.TreeItems;
using CloudFileServer.Services.Visitors;

namespace CloudFileServer.Tests.Services.Visitors;

public class SearchVisitorTests
{
    private static readonly DateTime Now = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static TextFileTreeItem MakeText(long id, string name) =>
        new(id, name, 100, $"/{name}.txt", "UTF-8", Array.Empty<Tag>(), Now, Now);

    private static ImageFileTreeItem MakeImage(long id, string name) =>
        new(id, name, 200, $"/{name}.jpg", 100, 100, Array.Empty<Tag>(), Now, Now);

    private static DirectoryTreeItem MakeDir(long id, string name, params NodeTreeItem[] children)
    {
        var dir = new DirectoryTreeItem(id, name, Array.Empty<Tag>(), Now, Now);
        foreach (var child in children) dir.AddChild(child);
        return dir;
    }

    [Fact]
    public void Search_ExactMatch_ReturnsNode()
    {
        var visitor = new SearchVisitor("readme");
        MakeText(1, "readme").Accept(visitor);
        Assert.Single(visitor.Results);
        Assert.Equal("readme", visitor.Results[0].Name);
    }

    [Fact]
    public void Search_PartialMatch_ReturnsNode()
    {
        var visitor = new SearchVisitor("read");
        MakeText(1, "readme").Accept(visitor);
        Assert.Single(visitor.Results);
    }

    [Fact]
    public void Search_CaseInsensitive_ReturnsNode()
    {
        var visitor = new SearchVisitor("README");
        MakeText(1, "readme").Accept(visitor);
        Assert.Single(visitor.Results);
    }

    [Fact]
    public void Search_NoMatch_ReturnsEmpty()
    {
        var visitor = new SearchVisitor("xyz_not_exist");
        MakeText(1, "readme").Accept(visitor);
        Assert.Empty(visitor.Results);
    }

    [Fact]
    public void Search_DirectoryNameMatches_IncludesDirectory()
    {
        var visitor = new SearchVisitor("docs");
        MakeDir(1, "docs", MakeText(2, "file")).Accept(visitor);
        Assert.Single(visitor.Results);
        Assert.True(visitor.Results[0].IsDirectory);
    }

    [Fact]
    public void Search_ImageFileMatches_IncludesImage()
    {
        var visitor = new SearchVisitor("photo");
        MakeImage(1, "photo").Accept(visitor);
        Assert.Single(visitor.Results);
        Assert.IsType<ImageFileTreeItem>(visitor.Results[0]);
    }

    [Fact]
    public void Search_MultipleMatches_ReturnsAll()
    {
        var visitor = new SearchVisitor("report");
        MakeDir(1, "docs",
            MakeText(2, "report_draft"),
            MakeText(3, "report_final"),
            MakeText(4, "readme")).Accept(visitor);
        Assert.Equal(2, visitor.Results.Count);
    }

    [Fact]
    public void Search_DeepNested_FindsMatchInSubdirectory()
    {
        var visitor = new SearchVisitor("deep");
        MakeDir(1, "root",
            MakeDir(2, "level1",
                MakeDir(3, "level2",
                    MakeText(4, "deep_file")))).Accept(visitor);
        Assert.Single(visitor.Results);
        Assert.Equal("deep_file", visitor.Results[0].Name);
    }
}
