using CloudFileServer.Domain.Models.Dtos;

namespace CloudFileServer.Services.Sorting;

public class TreeSortContext
{
    private readonly ISortStrategy _strategy;
    private readonly bool _ascending;

    public TreeSortContext(ISortStrategy strategy, bool ascending)
    {
        _strategy = strategy;
        _ascending = ascending;
    }

    public List<NodeTreeItemDto> Sort(IEnumerable<NodeTreeItemDto> nodes)
    {
        var sorted = _strategy.Sort(nodes, _ascending).ToList();
        foreach (var node in sorted)
        {
            if (node.Children is { Count: > 0 })
            {
                var sortedChildren = Sort(node.Children);
                node.Children.Clear();
                node.Children.AddRange(sortedChildren);
            }
        }
        return sorted;
    }
}
