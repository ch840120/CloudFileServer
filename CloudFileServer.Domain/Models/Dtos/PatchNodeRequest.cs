namespace CloudFileServer.Domain.Models.Dtos;

/// <summary>
/// PATCH /api/nodes/{id}
/// - isDeleted: false → restore soft-deleted node
/// </summary>
public class PatchNodeRequest
{
    public bool? IsDeleted { get; set; }
}
