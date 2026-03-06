using CloudFileServer.Domain.Models;

namespace CloudFileServer.Domain.Interfaces;

public interface ITagRepository
{
    Task<IReadOnlyList<Tag>> GetAllTagsAsync(CancellationToken cancellationToken = default);
    Task AddTagToNodeAsync(long nodeId, int tagId, CancellationToken cancellationToken = default);
    Task RemoveTagFromNodeAsync(long nodeId, int tagId, CancellationToken cancellationToken = default);
}
