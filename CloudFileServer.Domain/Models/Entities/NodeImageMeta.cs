using System.ComponentModel.DataAnnotations;

namespace CloudFileServer.Domain.Models;

public class NodeImageMeta
{
    [Key]
    public long NodeId { get; set; }

    public int WidthPx { get; set; }

    public int HeightPx { get; set; }

}
