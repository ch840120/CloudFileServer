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

    public async Task<CopyResult> CopySubtreeAsync(
        long sourceNodeId,
        long? targetParentId,
        CancellationToken cancellationToken = default)
    {
        // Round-trip 1: load all active nodes; resolve subtree membership and naming sets in memory.
        var allNodes = await _dbContext.Nodes.AsNoTracking()
            .Where(n => !n.IsDeleted)
            .ToListAsync(cancellationToken);

        var subtreeIds = new HashSet<long>();
        CollectSubtreeIds(allNodes, sourceNodeId, subtreeIds);
        var subtreeIdList = subtreeIds.ToList();

        var siblingNames = allNodes
            .Where(n => n.ParentId == targetParentId)
            .Select(n => n.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingPaths = allNodes
            .Where(n => n.StoragePath is not null)
            .Select(n => n.StoragePath!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        // Round-trip 2: LEFT JOINs for 1:0..1 metas; correlated subquery for 1:many tags.
        //               Tags cannot be LEFT JOINed directly — each tag row would multiply
        //               the node row, requiring a dedup pass. Correlated subquery avoids this.
        var subtreeData = await _dbContext.Nodes.AsNoTracking()
            .Where(n => !n.IsDeleted && subtreeIdList.Contains(n.Id))
            .Select(node => new
            {
                Node   = node,
                Img    = _dbContext.NodeImageMetas.FirstOrDefault(m => m.NodeId == node.Id),
                Txt    = _dbContext.NodeTextMetas.FirstOrDefault(m => m.NodeId == node.Id),
                Wrd    = _dbContext.NodeWordMetas.FirstOrDefault(m => m.NodeId == node.Id),
                TagIds = _dbContext.NodeTags.Where(t => t.NodeId == node.Id).Select(t => t.TagId).ToList()
            })
            .ToListAsync(cancellationToken);

        var subtreeMap = subtreeData.ToDictionary(d => d.Node.Id);
        var subtreeNodes = subtreeData.Select(d => d.Node).ToList();
        var sourceNode  = subtreeNodes.First(n => n.Id == sourceNodeId);
        string rootCopyName = GetUniqueCopyName(sourceNode.Name, siblingNames);
        var now = DateTime.UtcNow;

        var nodeMap      = new Dictionary<long, Node>();
        var fileMappings = new List<(string OldPath, string NewPath)>();

        // IO 3 prep: BFS builds all new Node objects with ParentId = null (set after IO 3).
        var queue = new Queue<Node>();
        queue.Enqueue(sourceNode);

        while (queue.Count > 0)
        {
            var original = queue.Dequeue();

            foreach (var child in subtreeNodes.Where(n => n.ParentId == original.Id))
                queue.Enqueue(child);

            string? newStoragePath = null;
            if (original.StoragePath is not null)
            {
                newStoragePath = GetUniqueCopyPath(original.StoragePath, existingPaths);
                existingPaths.Add(newStoragePath);
                fileMappings.Add((original.StoragePath, newStoragePath));
            }

            var newNode = new Node
            {
                NodeTypeId  = original.NodeTypeId,
                Name        = original.Id == sourceNodeId ? rootCopyName : original.Name,
                SizeBytes   = original.SizeBytes,
                StoragePath = newStoragePath,
                IsDeleted   = false,
                CreatedAt   = now,
                UpdatedAt   = now
            };

            _dbContext.Nodes.Add(newNode);
            nodeMap[original.Id] = newNode;
        }

        await _dbContext.SaveChangesAsync(cancellationToken); // IO 3: INSERT all new nodes (ParentId = null), EF back-fills IDENTITY Ids

        // Back-fill ParentId in memory — EF detects Modified, IO 4 will UPDATE
        foreach (var original in subtreeNodes)
        {
            nodeMap[original.Id].ParentId = original.Id == sourceNodeId
                ? targetParentId
                : nodeMap[original.ParentId!.Value].Id;
        }

        // IO 4: UPDATE ParentId + INSERT all metas/tags — one SaveChanges
        foreach (var (oldId, newNode) in nodeMap)
        {
            var d = subtreeMap[oldId];

            if (d.Img is not null)
                _dbContext.NodeImageMetas.Add(new NodeImageMeta { NodeId = newNode.Id, WidthPx = d.Img.WidthPx, HeightPx = d.Img.HeightPx });

            if (d.Txt is not null)
                _dbContext.NodeTextMetas.Add(new NodeTextMeta { NodeId = newNode.Id, Encoding = d.Txt.Encoding });

            if (d.Wrd is not null)
                _dbContext.NodeWordMetas.Add(new NodeWordMeta { NodeId = newNode.Id, PageCount = d.Wrd.PageCount });

            foreach (var tagId in d.TagIds)
                _dbContext.NodeTags.Add(new NodeTag { NodeId = newNode.Id, TagId = tagId });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CopyResult(nodeMap[sourceNodeId].Id, fileMappings.AsReadOnly());
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
