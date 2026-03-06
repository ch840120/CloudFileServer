using System.ComponentModel.DataAnnotations;

namespace CloudFileServer.Domain.Models;

public class NodeTextMeta
{
    [Key]
    public long NodeId { get; set; }

    [MaxLength(50)]
    public string Encoding { get; set; } = "UTF-8";

}
