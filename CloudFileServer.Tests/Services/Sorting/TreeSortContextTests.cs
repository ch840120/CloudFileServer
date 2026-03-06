using CloudFileServer.Domain.Models.Dtos;
using CloudFileServer.Services.Sorting;

namespace CloudFileServer.Tests.Services.Sorting;

public class TreeSortContextTests
{
    private static NodeTreeItemDto MakeFile(long id, string name, string storagePath, long sizeBytes) =>
        new() { Id = id, Name = name, StoragePath = storagePath, SizeBytes = sizeBytes, IsDirectory = false };

    private static NodeTreeItemDto MakeDir(long id, string name, List<NodeTreeItemDto>? children = null) =>
        new() { Id = id, Name = name, IsDirectory = true, Children = children ?? new List<NodeTreeItemDto>() };

    // ---- Name strategy ----

    [Fact]
    public void NameStrategy_Ascending_SortsAlphabetically()
    {
        var nodes = new List<NodeTreeItemDto>
        {
            MakeFile(1, "Zebra.txt", "z.txt", 100),
            MakeFile(2, "apple.txt", "a.txt", 200),
            MakeFile(3, "Mango.txt", "m.txt", 150),
        };
        var context = new TreeSortContext(new NameSortStrategy(), ascending: true);

        var result = context.Sort(nodes);

        Assert.Equal("apple.txt", result[0].Name);
        Assert.Equal("Mango.txt", result[1].Name);
        Assert.Equal("Zebra.txt", result[2].Name);
    }

    [Fact]
    public void NameStrategy_Descending_SortsReverseAlphabetically()
    {
        var nodes = new List<NodeTreeItemDto>
        {
            MakeFile(1, "apple.txt", "a.txt", 100),
            MakeFile(2, "Zebra.txt", "z.txt", 200),
        };
        var context = new TreeSortContext(new NameSortStrategy(), ascending: false);

        var result = context.Sort(nodes);

        Assert.Equal("Zebra.txt", result[0].Name);
        Assert.Equal("apple.txt", result[1].Name);
    }

    // ---- Size strategy ----

    [Fact]
    public void SizeStrategy_Ascending_SmallestFirst_DirectoriesLast()
    {
        var nodes = new List<NodeTreeItemDto>
        {
            MakeFile(1, "big.txt",   "big.txt",   5000),
            MakeDir(2,  "folder"),
            MakeFile(3, "small.txt", "small.txt",  100),
        };
        var context = new TreeSortContext(new SizeSortStrategy(), ascending: true);

        var result = context.Sort(nodes);

        Assert.Equal("small.txt", result[0].Name);
        Assert.Equal("big.txt",   result[1].Name);
        Assert.Equal("folder",    result[2].Name);
    }

    [Fact]
    public void SizeStrategy_Descending_LargestFirst_DirectoriesLast()
    {
        var nodes = new List<NodeTreeItemDto>
        {
            MakeFile(1, "small.txt", "small.txt", 100),
            MakeFile(2, "big.txt",   "big.txt",  5000),
            MakeDir(3,  "folder"),
        };
        var context = new TreeSortContext(new SizeSortStrategy(), ascending: false);

        var result = context.Sort(nodes);

        Assert.Equal("big.txt",   result[0].Name);
        Assert.Equal("small.txt", result[1].Name);
        Assert.Equal("folder",    result[2].Name);
    }

    // ---- Extension strategy ----

    [Fact]
    public void ExtensionStrategy_Ascending_DirectoriesFirst_ThenByExtension()
    {
        var nodes = new List<NodeTreeItemDto>
        {
            MakeFile(1, "report", "report.pdf", 500),
            MakeDir(2,  "docs"),
            MakeFile(3, "notes",  "notes.txt",  100),
        };
        var context = new TreeSortContext(new ExtensionSortStrategy(), ascending: true);

        var result = context.Sort(nodes);

        Assert.True(result[0].IsDirectory);
        Assert.Equal(".pdf", System.IO.Path.GetExtension(result[1].StoragePath));
        Assert.Equal(".txt", System.IO.Path.GetExtension(result[2].StoragePath));
    }

    // ---- Recursive sort ----

    [Fact]
    public void Sort_Recursive_AppliesStrategyToChildren()
    {
        var children = new List<NodeTreeItemDto>
        {
            MakeFile(10, "Zebra.txt", "z.txt", 100),
            MakeFile(11, "apple.txt", "a.txt", 200),
        };
        var nodes = new List<NodeTreeItemDto> { MakeDir(1, "root", children) };
        var context = new TreeSortContext(new NameSortStrategy(), ascending: true);

        var result = context.Sort(nodes);

        var resultChildren = result[0].Children!;
        Assert.Equal("apple.txt", resultChildren[0].Name);
        Assert.Equal("Zebra.txt", resultChildren[1].Name);
    }
}
