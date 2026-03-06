using CloudFileServer.Domain.Models.Dtos;

namespace CloudFileServer.Services.Sorting;

public interface ISortStrategy
{
    IEnumerable<NodeTreeItemDto> Sort(IEnumerable<NodeTreeItemDto> nodes, bool ascending);
}
