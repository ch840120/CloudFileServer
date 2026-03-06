using CloudFileServer.Domain.Models.Dtos;

namespace CloudFileServer.Services.Sorting;

public class NameSortStrategy : ISortStrategy
{
    public IEnumerable<NodeTreeItemDto> Sort(IEnumerable<NodeTreeItemDto> nodes, bool ascending)
    {
        return ascending
            ? nodes.OrderBy(n => n.Name, StringComparer.OrdinalIgnoreCase)
            : nodes.OrderByDescending(n => n.Name, StringComparer.OrdinalIgnoreCase);
    }
}
