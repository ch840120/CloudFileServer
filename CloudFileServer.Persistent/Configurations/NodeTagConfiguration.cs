using CloudFileServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudFileServer.Persistent.Configurations;

public class NodeTagConfiguration : IEntityTypeConfiguration<NodeTag>
{
    public void Configure(EntityTypeBuilder<NodeTag> builder)
    {
        builder.ToTable("NodeTags");

        builder.HasKey(e => new { e.NodeId, e.TagId });

        builder.HasOne<Node>()
            .WithMany()
            .HasForeignKey(e => e.NodeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(e => e.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
