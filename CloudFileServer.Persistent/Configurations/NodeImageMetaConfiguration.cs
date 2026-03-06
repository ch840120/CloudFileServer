using CloudFileServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudFileServer.Persistent.Configurations;

public class NodeImageMetaConfiguration : IEntityTypeConfiguration<NodeImageMeta>
{
    public void Configure(EntityTypeBuilder<NodeImageMeta> builder)
    {
        builder.ToTable("NodeImageMeta");

        builder.HasOne<Node>()
            .WithOne()
            .HasForeignKey<NodeImageMeta>(e => e.NodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
