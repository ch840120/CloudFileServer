using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudFileServer.Persistent.Repository;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _dbContext;

    public TagRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Tag>> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tags.AsNoTracking().OrderBy(t => t.Name).ToListAsync(cancellationToken);
    }

    public async Task AddTagToNodeAsync(long nodeId, int tagId, CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.NodeTags
            .AnyAsync(nt => nt.NodeId == nodeId && nt.TagId == tagId, cancellationToken);

        if (!exists)
        {
            _dbContext.NodeTags.Add(new NodeTag { NodeId = nodeId, TagId = tagId });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveTagFromNodeAsync(long nodeId, int tagId, CancellationToken cancellationToken = default)
    {
        var nodeTag = await _dbContext.NodeTags
            .FirstOrDefaultAsync(nt => nt.NodeId == nodeId && nt.TagId == tagId, cancellationToken);

        if (nodeTag is not null)
        {
            _dbContext.NodeTags.Remove(nodeTag);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
