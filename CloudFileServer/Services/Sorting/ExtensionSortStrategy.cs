using CloudFileServer.Domain.Models.Dtos;

namespace CloudFileServer.Services.Sorting;

public class ExtensionSortStrategy : ISortStrategy
{
    public IEnumerable<NodeTreeItemDto> Sort(IEnumerable<NodeTreeItemDto> nodes, bool ascending)
    {
        // Directories grouped first, then files sorted by extension
        return ascending
            ? nodes.OrderBy(n => n.IsDirectory ? 0 : 1)
                   .ThenBy(n => Path.GetExtension(n.StoragePath ?? string.Empty), StringComparer.OrdinalIgnoreCase)
            : nodes.OrderBy(n => n.IsDirectory ? 0 : 1)
                   .ThenByDescending(n => Path.GetExtension(n.StoragePath ?? string.Empty), StringComparer.OrdinalIgnoreCase);
    }
}
