using System.ComponentModel.DataAnnotations;

namespace CloudFileServer.Domain.Models;

public class NodeType
{
    [Key]
    public short Id { get; set; }

    public NodeTypeCode Code { get; set; }

    public bool IsLeaf { get; set; } = true;

    public DateTime CreatedAt { get; set; }

}
