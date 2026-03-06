using System.ComponentModel.DataAnnotations;

namespace CloudFileServer.Domain.Models;

public class NodeWordMeta
{
    [Key]
    public long NodeId { get; set; }

    public int PageCount { get; set; }

}
