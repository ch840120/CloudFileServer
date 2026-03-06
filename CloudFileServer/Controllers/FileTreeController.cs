using CloudFileServer.Applibs;
using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models;
using CloudFileServer.Domain.Models.Dtos;
using CloudFileServer.Domain.Models.TreeItems;
using CloudFileServer.Services.Sorting;
using CloudFileServer.Services.Visitors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CloudFileServer.Controllers;

[ApiController]
[Route("api/nodes")]
public class FileTreeController : ControllerBase
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly INodeTreeRepository _nodeTreeRepository;
    private readonly IMemoryCache _cache;

    public FileTreeController(INodeTreeRepository nodeTreeRepository, IMemoryCache cache)
    {
        _nodeTreeRepository = nodeTreeRepository;
        _cache = cache;
    }

    /// <summary>
    /// GET /api/nodes                      → 完整樹（可帶 sortBy / order）
    /// GET /api/nodes?q=keyword            → 關鍵字搜尋
    /// GET /api/nodes?ext=.docx            → 副檔名搜尋
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetNodes(
        [FromQuery] string? sortBy,
        [FromQuery] string? order,
        [FromQuery] string? q,
        [FromQuery] string? ext,
        CancellationToken cancellationToken)
    {
        var tree = await GetTreeAsync(cancellationToken);

        if (q is not null)
        {
            var inner   = new SearchVisitor(q);
            var visitor = new TraversalLogDecorator(inner);
            foreach (var root in tree) root.Accept(visitor);
            return Ok(new { results = inner.Results.Select(MapToDto).ToList(), traversalLog = visitor.Log });
        }

        if (ext is not null)
        {
            var inner   = new ExtensionSearchVisitor(ext);
            var visitor = new TraversalLogDecorator(inner);
            foreach (var root in tree) root.Accept(visitor);
            return Ok(new { results = inner.Results.Select(MapToDto).ToList(), traversalLog = visitor.Log });
        }

        var dtos = tree.Select(MapToDto).ToList();

        ISortStrategy? strategy = sortBy?.ToLowerInvariant() switch
        {
            "name"      => new NameSortStrategy(),
            "size"      => new SizeSortStrategy(),
            "extension" => new ExtensionSortStrategy(),
            _           => null
        };

        if (strategy is not null)
        {
            bool ascending = !string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
            return Ok(new TreeSortContext(strategy, ascending).Sort(dtos));
        }

        return Ok(dtos);
    }

    /// <summary>GET /api/nodes/size?nodeId=  → 計算容量</summary>
    [HttpGet("size")]
    public async Task<ActionResult<long>> GetSize(
        [FromQuery] long? nodeId,
        CancellationToken cancellationToken)
    {
        var tree   = await GetTreeAsync(cancellationToken);
        var target = nodeId.HasValue ? FindNodeById(tree, nodeId.Value) : null;
        IEnumerable<NodeTreeItem> roots = target is not null ? [target] : tree;

        var inner   = new CalculateSizeVisitor();
        var visitor = new TraversalLogDecorator(inner);
        foreach (var root in roots) root.Accept(visitor);
        return Ok(new { totalBytes = inner.TotalBytes, traversalLog = visitor.Log });
    }

    /// <summary>GET /api/nodes/export  → 匯出整棵樹為 XML</summary>
    [HttpGet("export")]
    public async Task<ActionResult<string>> Export(CancellationToken cancellationToken)
    {
        var tree    = await GetTreeAsync(cancellationToken);
        var visitor = new XmlSerializationVisitor();
        foreach (var root in tree) root.Accept(visitor);
        return Ok(visitor.GetXml());
    }

    private static NodeTreeItem? FindNodeById(IReadOnlyList<NodeTreeItem> nodes, long id)
    {
        foreach (var node in nodes)
        {
            if (node.Id == id) return node;
            if (node is DirectoryTreeItem dir)
            {
                var found = FindNodeById(dir.Children, id);
                if (found is not null) return found;
            }
        }
        return null;
    }

    private async Task<IReadOnlyList<NodeTreeItem>> GetTreeAsync(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(AppCacheKeys.FileTree, out IReadOnlyList<NodeTreeItem>? cached) && cached is not null)
            return cached;

        var tree = await _nodeTreeRepository.GetFullTreeAsync(cancellationToken);
        _cache.Set(AppCacheKeys.FileTree, tree, CacheDuration);
        return tree;
    }

    private static NodeTreeItemDto MapToDto(NodeTreeItem item)
    {
        var tags = item.Tags.Select(MapTag).ToList();

        return item switch
        {
            DirectoryTreeItem dir => new NodeTreeItemDto
            {
                Id          = dir.Id,
                Name        = dir.Name,
                TypeCode    = nameof(NodeTypeCode.Directory),
                IsDirectory = true,
                Tags        = tags,
                CreatedAt   = dir.CreatedAt,
                UpdatedAt   = dir.UpdatedAt,
                Children    = dir.Children.Select(MapToDto).ToList()
            },
            ImageFileTreeItem img => new NodeTreeItemDto
            {
                Id             = img.Id,
                Name           = img.Name,
                TypeCode       = nameof(NodeTypeCode.Image),
                IsDirectory    = false,
                Tags           = tags,
                CreatedAt      = img.CreatedAt,
                UpdatedAt      = img.UpdatedAt,
                SizeBytes      = img.SizeBytes,
                SizeFormatted  = FormatBytes(img.SizeBytes),
                StoragePath    = img.StoragePath,
                WidthPx        = img.WidthPx,
                HeightPx       = img.HeightPx
            },
            TextFileTreeItem txt => new NodeTreeItemDto
            {
                Id            = txt.Id,
                Name          = txt.Name,
                TypeCode      = nameof(NodeTypeCode.Text),
                IsDirectory   = false,
                Tags          = tags,
                CreatedAt     = txt.CreatedAt,
                UpdatedAt     = txt.UpdatedAt,
                SizeBytes     = txt.SizeBytes,
                SizeFormatted = FormatBytes(txt.SizeBytes),
                StoragePath   = txt.StoragePath,
                Encoding      = txt.Encoding
            },
            WordFileTreeItem wrd => new NodeTreeItemDto
            {
                Id            = wrd.Id,
                Name          = wrd.Name,
                TypeCode      = nameof(NodeTypeCode.Word),
                IsDirectory   = false,
                Tags          = tags,
                CreatedAt     = wrd.CreatedAt,
                UpdatedAt     = wrd.UpdatedAt,
                SizeBytes     = wrd.SizeBytes,
                SizeFormatted = FormatBytes(wrd.SizeBytes),
                StoragePath   = wrd.StoragePath,
                PageCount     = wrd.PageCount
            },
            _ => throw new InvalidOperationException(
                $"Unknown NodeTreeItem type: {item.GetType().Name} for Id: {item.Id}")
        };
    }

    private static string FormatBytes(long bytes) => bytes switch
    {
        < 1024           => $"{bytes} B",
        < 1024 * 1024    => $"{bytes / 1024.0:0.#} KB",
        _                => $"{bytes / (1024.0 * 1024):0.#} MB"
    };

    private static TagDto MapTag(Tag tag) => new()
    {
        Id    = tag.Id,
        Name  = tag.Name,
        Color = tag.Color
    };
}
