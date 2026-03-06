namespace CloudFileServer.Domain.Interfaces;

public record CopyResult(long NewRootNodeId, IReadOnlyList<(string OldPath, string NewPath)> FileMappings);

public interface INodeEditRepository
{
    Task SoftDeleteAsync(long nodeId, CancellationToken cancellationToken = default);
    Task RestoreAsync(long nodeId, CancellationToken cancellationToken = default);
    Task<CopyResult> CopySubtreeAsync(long sourceNodeId, long? targetParentId, CancellationToken cancellationToken = default);
}
