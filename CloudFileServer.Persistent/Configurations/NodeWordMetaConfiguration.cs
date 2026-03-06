using CloudFileServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudFileServer.Persistent.Configurations;

public class NodeWordMetaConfiguration : IEntityTypeConfiguration<NodeWordMeta>
{
    public void Configure(EntityTypeBuilder<NodeWordMeta> builder)
    {
        builder.ToTable("NodeWordMeta");

        builder.HasOne<Node>()
            .WithOne()
            .HasForeignKey<NodeWordMeta>(e => e.NodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
