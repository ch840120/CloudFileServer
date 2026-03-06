using CloudFileServer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudFileServer.Persistent.Configurations;

public class NodeTypeConfiguration : IEntityTypeConfiguration<NodeType>
{
    public void Configure(EntityTypeBuilder<NodeType> builder)
    {
        builder.ToTable("NodeTypes");

        builder.Property(e => e.Code)
            .HasConversion<short>();
    }
}
