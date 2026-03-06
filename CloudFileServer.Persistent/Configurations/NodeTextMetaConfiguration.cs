using CloudFileServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudFileServer.Persistent.Configurations;

public class NodeTextMetaConfiguration : IEntityTypeConfiguration<NodeTextMeta>
{
    public void Configure(EntityTypeBuilder<NodeTextMeta> builder)
    {
        builder.ToTable("NodeTextMeta");

        builder.HasOne<Node>()
            .WithOne()
            .HasForeignKey<NodeTextMeta>(e => e.NodeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
