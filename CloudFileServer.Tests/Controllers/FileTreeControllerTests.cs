using CloudFileServer.Controllers;
using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models;
using CloudFileServer.Domain.Models.Dtos;
using CloudFileServer.Domain.Models.TreeItems;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace CloudFileServer.Tests.Controllers;

public class FileTreeControllerTests : IDisposable
{
    private readonly Mock<INodeTreeRepository> _repoMock = new();
    private readonly IMemoryCache              _cache    = new MemoryCache(new MemoryCacheOptions());
    private readonly FileTreeController        _sut;

    private static readonly DateTime Now = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public FileTreeControllerTests()
    {
        _sut = new FileTreeController(_repoMock.Object, _cache);
    }

    public void Dispose() => _cache.Dispose();

    // ── Helpers ─────────────────────────────────────────────────────────

    private static TextFileTreeItem MakeText(long id, string name, string path, long size = 500) =>
        new(id, name, size, path, "UTF-8", Array.Empty<Tag>(), Now, Now);

    private static ImageFileTreeItem MakeImage(long id, string name, string path, long size = 200) =>
        new(id, name, size, path, 800, 600, Array.Empty<Tag>(), Now, Now);

    private static WordFileTreeItem MakeWord(long id, string name, string path, long size = 1024) =>
        new(id, name, size, path, 10, Array.Empty<Tag>(), Now, Now);

    private static DirectoryTreeItem MakeDir(long id, string name, params NodeTreeItem[] children)
    {
        var dir = new DirectoryTreeItem(id, name, Array.Empty<Tag>(), Now, Now);
        foreach (var child in children) dir.AddChild(child);
        return dir;
    }

    private IReadOnlyList<NodeTreeItem> SimpleTree() =>
        new List<NodeTreeItem>
        {
            MakeDir(1, "docs",
                MakeText(2, "readme", "/docs/readme.txt", 100),
                MakeWord(3, "report", "/docs/report.docx", 2048)),
            MakeImage(4, "photo", "/photo.jpg", 300)
        };

    // ── GetNodes — full tree ─────────────────────────────────────────────

    [Fact]
    public async Task GetNodes_NoParams_ReturnsAllNodesAsDto()
    {
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(SimpleTree());

        var result = await _sut.GetNodes(null, null, null, null, default);

        var ok   = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<NodeTreeItemDto>>(ok.Value).ToList();
        Assert.Equal(2, list.Count);                     // docs dir + photo
        Assert.True(list[0].IsDirectory);
        Assert.Equal("photo", list[1].Name);
    }

    [Fact]
    public async Task GetNodes_CacheHit_SkipsRepository()
    {
        var tree = SimpleTree();
        _cache.Set("file-tree", tree);

        await _sut.GetNodes(null, null, null, null, default);

        _repoMock.Verify(r => r.GetFullTreeAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetNodes_CacheMiss_CallsRepositoryAndPopulatesCache()
    {
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(SimpleTree());

        await _sut.GetNodes(null, null, null, null, default);

        _repoMock.Verify(r => r.GetFullTreeAsync(default), Times.Once);
        Assert.True(_cache.TryGetValue("file-tree", out _));
    }

    // ── GetNodes — sorting ───────────────────────────────────────────────

    [Fact]
    public async Task GetNodes_SortByName_Asc_ReturnsDtosAlphabetically()
    {
        var tree = new List<NodeTreeItem>
        {
            MakeText(1, "zebra", "/z.txt"),
            MakeText(2, "apple", "/a.txt"),
            MakeText(3, "mango", "/m.txt"),
        };
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(tree);

        var result = await _sut.GetNodes("name", "asc", null, null, default);

        var ok   = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<NodeTreeItemDto>>(ok.Value).ToList();
        Assert.Equal("apple", list[0].Name);
        Assert.Equal("mango", list[1].Name);
        Assert.Equal("zebra", list[2].Name);
    }

    [Fact]
    public async Task GetNodes_SortBySize_Desc_LargestFirst()
    {
        var tree = new List<NodeTreeItem>
        {
            MakeText(1, "small", "/s.txt", 100),
            MakeText(2, "large", "/l.txt", 9999),
            MakeText(3, "mid",   "/m.txt", 500),
        };
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(tree);

        var result = await _sut.GetNodes("size", "desc", null, null, default);

        var ok   = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<NodeTreeItemDto>>(ok.Value).ToList();
        Assert.Equal("large", list[0].Name);
        Assert.Equal("mid",   list[1].Name);
        Assert.Equal("small", list[2].Name);
    }

    [Fact]
    public async Task GetNodes_UnknownSortBy_ReturnsUnsorted()
    {
        var tree = new List<NodeTreeItem>
        {
            MakeText(1, "b", "/b.txt"),
            MakeText(2, "a", "/a.txt"),
        };
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(tree);

        var result = await _sut.GetNodes("unknown", "asc", null, null, default);

        var ok   = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<NodeTreeItemDto>>(ok.Value).ToList();
        Assert.Equal("b", list[0].Name); // original order preserved
    }

    // ── GetNodes — keyword search ────────────────────────────────────────

    // Anonymous-type result: access properties via reflection to avoid
    // JsonSerializer<object> stripping away runtime-type properties.
    private static (IReadOnlyList<NodeTreeItemDto> results, IReadOnlyList<string> log)
        ExtractSearchResult(object? value)
    {
        var type    = value!.GetType();
        var results = (IEnumerable<NodeTreeItemDto>)type.GetProperty("results")!.GetValue(value)!;
        var log     = (IReadOnlyList<string>)type.GetProperty("traversalLog")!.GetValue(value)!;
        return (results.ToList(), log);
    }

    [Fact]
    public async Task GetNodes_WithQuery_ReturnsMatchingNodesAndLog()
    {
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(SimpleTree());

        var result = await _sut.GetNodes(null, null, "readme", null, default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var (results, log) = ExtractSearchResult(ok.Value);
        Assert.Single(results);
        Assert.Equal("readme", results[0].Name);
        Assert.NotEmpty(log);
    }

    [Fact]
    public async Task GetNodes_WithQuery_NoMatch_ReturnsEmptyResults()
    {
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(SimpleTree());

        var result = await _sut.GetNodes(null, null, "nonexistent_xyz", null, default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var (results, _) = ExtractSearchResult(ok.Value);
        Assert.Empty(results);
    }

    // ── GetNodes — extension search ──────────────────────────────────────

    [Fact]
    public async Task GetNodes_WithExt_ReturnsOnlyMatchingExtension()
    {
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(SimpleTree());

        var result = await _sut.GetNodes(null, null, null, ".docx", default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var (results, _) = ExtractSearchResult(ok.Value);
        Assert.Single(results);
        Assert.Equal("report", results[0].Name);
    }

    [Fact]
    public async Task GetNodes_WithExtNoDot_StillMatchesCorrectly()
    {
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(SimpleTree());

        // "txt" without leading dot — ExtensionSearchVisitor normalises it
        var result = await _sut.GetNodes(null, null, null, "txt", default);

        var ok = Assert.IsType<OkObjectResult>(result);
        var (results, _) = ExtractSearchResult(ok.Value);
        Assert.Single(results);
    }

    // ── GetSize ──────────────────────────────────────────────────────────

    private static long ExtractTotalBytes(object? value)
    {
        var prop = value!.GetType().GetProperty("totalBytes")!;
        return (long)prop.GetValue(value)!;
    }

    [Fact]
    public async Task GetSize_NoNodeId_ReturnsSumOfAllFiles()
    {
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(SimpleTree());

        var result = await _sut.GetSize(null, default);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        // readme(100) + report(2048) + photo(300) = 2448
        Assert.Equal(2448, ExtractTotalBytes(ok.Value));
    }

    [Fact]
    public async Task GetSize_WithNodeId_ReturnsSubtreeSum()
    {
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(SimpleTree());

        // nodeId=1 is "docs" dir containing readme(100) + report(2048)
        var result = await _sut.GetSize(1, default);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(2148, ExtractTotalBytes(ok.Value));
    }

    [Fact]
    public async Task GetSize_NodeIdNotFound_ReturnsTotalTreeSize()
    {
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(SimpleTree());

        // nodeId=999 does not exist → falls back to whole tree
        var result = await _sut.GetSize(999, default);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(2448, ExtractTotalBytes(ok.Value));
    }

    // ── Export ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Export_ReturnsNonEmptyXmlString()
    {
        // XmlSerializationVisitor requires a single root element;
        // wrap all content under one root directory.
        var tree = new List<NodeTreeItem>
        {
            MakeDir(1, "root",
                MakeText(2, "readme", "/root/readme.txt"),
                MakeImage(3, "photo", "/root/photo.jpg"))
        };
        _repoMock.Setup(r => r.GetFullTreeAsync(default)).ReturnsAsync(tree);

        var result = await _sut.Export(default);

        var ok  = Assert.IsType<OkObjectResult>(result.Result);
        var xml = Assert.IsType<string>(ok.Value);
        Assert.False(string.IsNullOrWhiteSpace(xml));
        Assert.Contains("root", xml);
        Assert.Contains("readme", xml);
        Assert.Contains("photo", xml);
    }
}
