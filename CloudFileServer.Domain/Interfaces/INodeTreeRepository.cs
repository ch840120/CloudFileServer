using CloudFileServer.Domain.Models.TreeItems;

namespace CloudFileServer.Domain.Interfaces;

public interface INodeTreeRepository
{
    Task<IReadOnlyList<NodeTreeItem>> GetFullTreeAsync(CancellationToken cancellationToken = default);
}
