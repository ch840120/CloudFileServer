using CloudFileServer.Applibs;
using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CloudFileServer.Controllers;

[ApiController]
[Route("api/nodes")]
public class NodesController : ControllerBase
{
    private readonly INodeEditRepository _nodeEditRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ITagRepository      _tagRepository;
    private readonly IMemoryCache        _cache;

    public NodesController(
        INodeEditRepository nodeEditRepository,
        IFileStorageService fileStorageService,
        ITagRepository tagRepository,
        IMemoryCache cache)
    {
        _nodeEditRepository = nodeEditRepository;
        _fileStorageService = fileStorageService;
        _tagRepository      = tagRepository;
        _cache              = cache;
    }

    /// <summary>DELETE /api/nodes/{id}  → Soft-delete 節點</summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> SoftDelete(long id, CancellationToken cancellationToken)
    {
        await _nodeEditRepository.SoftDeleteAsync(id, cancellationToken);
        _cache.Remove(AppCacheKeys.FileTree);
        return NoContent();
    }

    /// <summary>POST /api/nodes/{id}/copies  → 複製節點（含子樹 + 實體檔）</summary>
    [HttpPost("{id:long}/copies")]
    public async Task<ActionResult<NodeOperationResponse>> Copy(
        long id,
        [FromBody] CopyNodeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _nodeEditRepository.CopySubtreeAsync(
            id, request.TargetParentId, cancellationToken);

        foreach (var (oldPath, newPath) in result.FileMappings)
            _fileStorageService.CopyFile(oldPath, newPath);

        _cache.Remove(AppCacheKeys.FileTree);
        return Ok(new NodeOperationResponse { NewNodeId = result.NewRootNodeId });
    }

    /// <summary>PATCH /api/nodes/{id}  body: { "isDeleted": false } → 還原 soft-deleted 節點</summary>
    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Patch(
        long id,
        [FromBody] PatchNodeRequest request,
        CancellationToken cancellationToken)
    {
        if (request.IsDeleted == false)
            await _nodeEditRepository.RestoreAsync(id, cancellationToken);

        _cache.Remove(AppCacheKeys.FileTree);
        return NoContent();
    }

    /// <summary>POST /api/nodes/{nodeId}/tags  → 新增標籤至節點</summary>
    [HttpPost("{nodeId:long}/tags")]
    public async Task<IActionResult> AddTag(
        long nodeId,
        [FromBody] AddTagRequest request,
        CancellationToken cancellationToken)
    {
        await _tagRepository.AddTagToNodeAsync(nodeId, request.TagId, cancellationToken);
        _cache.Remove(AppCacheKeys.FileTree);
        return NoContent();
    }

    /// <summary>DELETE /api/nodes/{nodeId}/tags/{tagId}  → 移除節點標籤</summary>
    [HttpDelete("{nodeId:long}/tags/{tagId:int}")]
    public async Task<IActionResult> RemoveTag(
        long nodeId,
        int tagId,
        CancellationToken cancellationToken)
    {
        await _tagRepository.RemoveTagFromNodeAsync(nodeId, tagId, cancellationToken);
        _cache.Remove(AppCacheKeys.FileTree);
        return NoContent();
    }
}
