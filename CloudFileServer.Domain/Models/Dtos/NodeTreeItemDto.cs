namespace CloudFileServer.Domain.Models.Dtos;

public class NodeTreeItemDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string TypeCode { get; init; } = string.Empty;
    public bool IsDirectory { get; init; }
    public List<TagDto> Tags { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    // Directory only
    public List<NodeTreeItemDto>? Children { get; init; }
    // File common
    public long? SizeBytes { get; init; }
    public string? StoragePath { get; init; }
    // Image only
    public int? WidthPx { get; init; }
    public int? HeightPx { get; init; }
    // Text only
    public string? Encoding { get; init; }
    // Word only
    public int? PageCount { get; init; }
}

public class TagDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
}
