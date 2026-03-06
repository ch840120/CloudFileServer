using System.ComponentModel.DataAnnotations;

namespace CloudFileServer.Domain.Models;

public class Node
{
    [Key]
    public long Id { get; set; }

    public short NodeTypeId { get; set; }

    public long? ParentId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    public long? SizeBytes { get; set; }

    [MaxLength(500)]
    public string? StoragePath { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}
