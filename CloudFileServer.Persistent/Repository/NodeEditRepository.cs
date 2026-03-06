using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudFileServer.Persistent.Repository;

public class NodeEditRepository : INodeEditRepository
{
    private readonly AppDbContext _dbContext;

    public NodeEditRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SoftDeleteAsync(long nodeId, CancellationToken cancellationToken = default)
    {
        var node = await _dbContext.Nodes.FindAsync(new object[] { nodeId }, cancellationToken)
            ?? throw new InvalidOperationException($"Node {nodeId} not found.");

        node.IsDeleted = true;
        node.DeletedAt = DateTime.UtcNow;
        node.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(long nodeId, CancellationToken cancellationToken = default)
    {
        var node = await _dbContext.Nodes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(n => n.Id == nodeId, cancellationToken)
            ?? throw new InvalidOperationException($"Node {nodeId} not found.");

        node.IsDeleted = false;
        node.DeletedAt = null;
        node.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task MoveAsync(long nodeId, long? newParentId, CancellationToken cancellationToken = default)
    {
        var node = await _dbContext.Nodes.FindAsync(new object[] { nodeId }, cancellationToken)
            ?? throw new InvalidOperationException($"Node {nodeId} not found.");

        node.ParentId = newParentId;
        node.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CopyResult> CopySubtreeAsync(
        long sourceNodeId,
        long? targetParentId,
        CancellationToken cancellationToken = default)
    {
        var allNodes = await _dbContext.Nodes.AsNoTracking()
            .Where(n => !n.IsDeleted)
            .ToListAsync(cancellationToken);

        var subtreeIds = new HashSet<long>();
        CollectSubtreeIds(allNodes, sourceNodeId, subtreeIds);

        var subtreeNodes = allNodes.Where(n => subtreeIds.Contains(n.Id)).ToList();
        var now = DateTime.UtcNow;

        // Build sets to generate unique names/paths without timestamp
        var siblingNames = allNodes
            .Where(n => n.ParentId == targetParentId)
            .Select(n => n.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingPaths = allNodes
            .Where(n => n.StoragePath is not null)
            .Select(n => n.StoragePath!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var sourceNode   = subtreeNodes.First(n => n.Id == sourceNodeId);
        string rootCopyName = GetUniqueCopyName(sourceNode.Name, siblingNames);

        var idMapping    = new Dictionary<long, long>();
        var fileMappings = new List<(string OldPath, string NewPath)>();

        // BFS so parents are always created before children
        var queue = new Queue<Node>();
        queue.Enqueue(sourceNode);

        while (queue.Count > 0)
        {
            var original = queue.Dequeue();

            foreach (var child in subtreeNodes.Where(n => n.ParentId == original.Id))
                queue.Enqueue(child);

            long? newParentId = original.Id == sourceNodeId
                ? targetParentId
                : idMapping[original.ParentId!.Value];

            string? newStoragePath = null;
            if (original.StoragePath is not null)
            {
                newStoragePath = GetUniqueCopyPath(original.StoragePath, existingPaths);
                existingPaths.Add(newStoragePath); // reserve so subsequent nodes don't collide
                fileMappings.Add((original.StoragePath, newStoragePath));
            }

            var newNode = new Node
            {
                NodeTypeId  = original.NodeTypeId,
                ParentId    = newParentId,
                Name        = original.Id == sourceNodeId ? rootCopyName : original.Name,
                SizeBytes   = original.SizeBytes,
                StoragePath = newStoragePath,
                IsDeleted   = false,
                CreatedAt   = now,
                UpdatedAt   = now
            };

            _dbContext.Nodes.Add(newNode);
            await _dbContext.SaveChangesAsync(cancellationToken); // required to get IDENTITY Id

            idMapping[original.Id] = newNode.Id;
        }

        // Copy meta records and tags
        foreach (var (oldId, newId) in idMapping)
        {
            var imgMeta = await _dbContext.NodeImageMetas.FindAsync(new object[] { oldId }, cancellationToken);
            if (imgMeta is not null)
                _dbContext.NodeImageMetas.Add(new NodeImageMeta { NodeId = newId, WidthPx = imgMeta.WidthPx, HeightPx = imgMeta.HeightPx });

            var txtMeta = await _dbContext.NodeTextMetas.FindAsync(new object[] { oldId }, cancellationToken);
            if (txtMeta is not null)
                _dbContext.NodeTextMetas.Add(new NodeTextMeta { NodeId = newId, Encoding = txtMeta.Encoding });

            var wrdMeta = await _dbContext.NodeWordMetas.FindAsync(new object[] { oldId }, cancellationToken);
            if (wrdMeta is not null)
                _dbContext.NodeWordMetas.Add(new NodeWordMeta { NodeId = newId, PageCount = wrdMeta.PageCount });

            // Copy tags
            var tagIds = await _dbContext.NodeTags
                .AsNoTracking()
                .Where(nt => nt.NodeId == oldId)
                .Select(nt => nt.TagId)
                .ToListAsync(cancellationToken);
            foreach (var tagId in tagIds)
                _dbContext.NodeTags.Add(new NodeTag { NodeId = newId, TagId = tagId });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CopyResult(idMapping[sourceNodeId], fileMappings.AsReadOnly());
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    private static string GetUniqueCopyName(string name, HashSet<string> existingNames)
    {
        string candidate = $"{name}_copy";
        if (!existingNames.Contains(candidate)) return candidate;
        int counter = 2;
        while (existingNames.Contains($"{name}_copy_{counter}"))
            counter++;
        return $"{name}_copy_{counter}";
    }

    private static string GetUniqueCopyPath(string originalPath, HashSet<string> existingPaths)
    {
        var dir       = Path.GetDirectoryName(originalPath)?.Replace('\\', '/') ?? string.Empty;
        var nameNoExt = Path.GetFileNameWithoutExtension(originalPath);
        var ext       = Path.GetExtension(originalPath);

        string MakePath(string n) =>
            string.IsNullOrEmpty(dir) ? $"{n}{ext}" : $"{dir}/{n}{ext}";

        string candidate = $"{nameNoExt}_copy";
        if (!existingPaths.Contains(MakePath(candidate))) return MakePath(candidate);
        int counter = 2;
        while (existingPaths.Contains(MakePath($"{nameNoExt}_copy_{counter}")))
            counter++;
        return MakePath($"{nameNoExt}_copy_{counter}");
    }

    private static void CollectSubtreeIds(List<Node> allNodes, long rootId, HashSet<long> result)
    {
        result.Add(rootId);
        foreach (var child in allNodes.Where(n => n.ParentId == rootId))
            CollectSubtreeIds(allNodes, child.Id, result);
    }
}
