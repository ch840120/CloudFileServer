using CloudFileServer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CloudFileServer.Persistent;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<NodeType> NodeTypes => Set<NodeType>();
    public DbSet<Node> Nodes => Set<Node>();
    public DbSet<NodeWordMeta> NodeWordMetas => Set<NodeWordMeta>();
    public DbSet<NodeImageMeta> NodeImageMetas => Set<NodeImageMeta>();
    public DbSet<NodeTextMeta> NodeTextMetas => Set<NodeTextMeta>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<NodeTag> NodeTags => Set<NodeTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
