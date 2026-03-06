using CloudFileServer.Domain.Models.Dtos;

namespace CloudFileServer.Services.Sorting;

public class SizeSortStrategy : ISortStrategy
{
    public IEnumerable<NodeTreeItemDto> Sort(IEnumerable<NodeTreeItemDto> nodes, bool ascending)
    {
        // Directories (null SizeBytes) always sort last
        return ascending
            ? nodes.OrderBy(n => n.SizeBytes.HasValue ? 0 : 1)
                   .ThenBy(n => n.SizeBytes ?? long.MaxValue)
            : nodes.OrderBy(n => n.SizeBytes.HasValue ? 0 : 1)
                   .ThenByDescending(n => n.SizeBytes ?? long.MinValue);
    }
}
