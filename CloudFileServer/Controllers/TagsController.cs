using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CloudFileServer.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly ITagRepository _tagRepository;

    public TagsController(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    /// <summary>GET /api/tags  → 取得所有可用標籤</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TagDto>>> GetAllTags(CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.GetAllTagsAsync(cancellationToken);
        return Ok(tags.Select(t => new TagDto { Id = t.Id, Name = t.Name, Color = t.Color }).ToList());
    }
}
