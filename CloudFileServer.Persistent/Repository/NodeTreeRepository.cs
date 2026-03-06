using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models;
using CloudFileServer.Domain.Models.TreeItems;
using Microsoft.EntityFrameworkCore;

namespace CloudFileServer.Persistent;

public class NodeTreeRepository : INodeTreeRepository
{
    private readonly AppDbContext _dbContext;

    public NodeTreeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private sealed record NodeFlatRow(
        long NodeId,
        long? ParentId,
        string Name,
        NodeTypeCode TypeCode,
        long? SizeBytes,
        string? StoragePath,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        int? ImageWidthPx,
        int? ImageHeightPx,
        string? TextEncoding,
        int? WordPageCount);

    public async Task<IReadOnlyList<NodeTreeItem>> GetFullTreeAsync(CancellationToken cancellationToken = default)
    {
        // Query 1: Node + NodeType + Meta tables (LEFT JOINs via GroupJoin+SelectMany)
        var rows = await _dbContext.Nodes.AsNoTracking()
            .Where(n => !n.IsDeleted)
            .Join(_dbContext.NodeTypes,
                n => n.NodeTypeId,
                nt => nt.Id,
                (n, nt) => new { n, TypeCode = nt.Code })
            .GroupJoin(_dbContext.NodeImageMetas.AsNoTracking(),
                x => x.n.Id,
                img => img.NodeId,
                (x, imgs) => new { x.n, x.TypeCode, imgs })
            .SelectMany(x => x.imgs.DefaultIfEmpty(),
                (x, img) => new { x.n, x.TypeCode, img })
            .GroupJoin(_dbContext.NodeTextMetas.AsNoTracking(),
                x => x.n.Id,
                txt => txt.NodeId,
                (x, txts) => new { x.n, x.TypeCode, x.img, txts })
            .SelectMany(x => x.txts.DefaultIfEmpty(),
                (x, txt) => new { x.n, x.TypeCode, x.img, txt })
            .GroupJoin(_dbContext.NodeWordMetas.AsNoTracking(),
                x => x.n.Id,
                wrd => wrd.NodeId,
                (x, wrds) => new { x.n, x.TypeCode, x.img, x.txt, wrds })
            .SelectMany(x => x.wrds.DefaultIfEmpty(),
                (x, wrd) => new { x.n, x.TypeCode, x.img, x.txt, wrd })
            .Select(x => new NodeFlatRow(
                x.n.Id,
                x.n.ParentId,
                x.n.Name,
                x.TypeCode,
                x.n.SizeBytes,
                x.n.StoragePath,
                x.n.CreatedAt,
                x.n.UpdatedAt,
                x.img != null ? x.img.WidthPx : (int?)null,
                x.img != null ? x.img.HeightPx : (int?)null,
                x.txt != null ? x.txt.Encoding : null,
                x.wrd != null ? x.wrd.PageCount : (int?)null))
            .ToListAsync(cancellationToken);

        var nodeIds = rows.Select(r => r.NodeId).ToHashSet();

        // Query 2: NodeTags JOIN Tags, filtered to nodes from Query 1
        var tagRows = await _dbContext.NodeTags.AsNoTracking()
            .Where(nt => nodeIds.Contains(nt.NodeId))
            .Join(_dbContext.Tags,
                nt => nt.TagId,
                t => t.Id,
                (nt, t) => new { nt.NodeId, Tag = t })
            .ToListAsync(cancellationToken);

        var tagsByNodeId = tagRows
            .GroupBy(x => x.NodeId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Tag).ToList());

        return BuildTree(rows, tagsByNodeId);
    }

    private static IReadOnlyList<NodeTreeItem> BuildTree(
        List<NodeFlatRow> rows,
        Dictionary<long, List<Tag>> tagsByNodeId)
    {
        var itemMap = new Dictionary<long, NodeTreeItem>(rows.Count);

        foreach (var row in rows)
        {
            var tags = tagsByNodeId.TryGetValue(row.NodeId, out var tagList)
                ? (IReadOnlyList<Tag>)tagList
                : Array.Empty<Tag>();

            itemMap[row.NodeId] = BuildTreeItem(row, tags);
        }

        var roots = new List<NodeTreeItem>();

        foreach (var row in rows)
        {
            if (row.ParentId is null)
            {
                roots.Add(itemMap[row.NodeId]);
            }
            else if (itemMap.TryGetValue(row.ParentId.Value, out var parent) &&
                     parent is DirectoryTreeItem directory)
            {
                directory.AddChild(itemMap[row.NodeId]);
            }
            // Orphan node (parent soft-deleted): silently skip
        }

        return roots;
    }

    private static NodeTreeItem BuildTreeItem(NodeFlatRow row, IReadOnlyList<Tag> tags)
    {
        return row.TypeCode switch
        {
            NodeTypeCode.Directory => new DirectoryTreeItem(
                row.NodeId,
                row.Name,
                tags,
                row.CreatedAt,
                row.UpdatedAt),

            NodeTypeCode.Image => new ImageFileTreeItem(
                row.NodeId,
                row.Name,
                row.SizeBytes!.Value,
                row.StoragePath!,
                row.ImageWidthPx!.Value,
                row.ImageHeightPx!.Value,
                tags,
                row.CreatedAt,
                row.UpdatedAt),

            NodeTypeCode.Text => new TextFileTreeItem(
                row.NodeId,
                row.Name,
                row.SizeBytes!.Value,
                row.StoragePath!,
                row.TextEncoding!,
                tags,
                row.CreatedAt,
                row.UpdatedAt),

            NodeTypeCode.Word => new WordFileTreeItem(
                row.NodeId,
                row.Name,
                row.SizeBytes!.Value,
                row.StoragePath!,
                row.WordPageCount!.Value,
                tags,
                row.CreatedAt,
                row.UpdatedAt),

            _ => throw new InvalidOperationException(
                $"Unknown NodeTypeCode: {row.TypeCode} for NodeId: {row.NodeId}")
        };
    }
}
