using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models;
using CloudFileServer.Domain.Models.Dtos;
using CloudFileServer.Domain.Models.TreeItems;
using Microsoft.AspNetCore.Mvc;

namespace CloudFileServer.Controllers;

[ApiController]
[Route("api/file-tree")]
public class FileTreeController : ControllerBase
{
    private readonly INodeTreeRepository _nodeTreeRepository;

    public FileTreeController(INodeTreeRepository nodeTreeRepository)
    {
        _nodeTreeRepository = nodeTreeRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NodeTreeItemDto>>> GetFullTree(
        CancellationToken cancellationToken)
    {
        var tree = await _nodeTreeRepository.GetFullTreeAsync(cancellationToken);
        var dtos = tree.Select(MapToDto).ToList();
        return Ok(dtos);
    }

    private static NodeTreeItemDto MapToDto(NodeTreeItem item)
    {
        var tags = item.Tags.Select(MapTag).ToList();

        return item switch
        {
            DirectoryTreeItem dir => new NodeTreeItemDto
            {
                Id = dir.Id,
                Name = dir.Name,
                TypeCode = nameof(NodeTypeCode.Directory),
                IsDirectory = true,
                Tags = tags,
                CreatedAt = dir.CreatedAt,
                UpdatedAt = dir.UpdatedAt,
                Children = dir.Children.Select(MapToDto).ToList()
            },
            ImageFileTreeItem img => new NodeTreeItemDto
            {
                Id = img.Id,
                Name = img.Name,
                TypeCode = nameof(NodeTypeCode.Image),
                IsDirectory = false,
                Tags = tags,
                CreatedAt = img.CreatedAt,
                UpdatedAt = img.UpdatedAt,
                SizeBytes = img.SizeBytes,
                StoragePath = img.StoragePath,
                WidthPx = img.WidthPx,
                HeightPx = img.HeightPx
            },
            TextFileTreeItem txt => new NodeTreeItemDto
            {
                Id = txt.Id,
                Name = txt.Name,
                TypeCode = nameof(NodeTypeCode.Text),
                IsDirectory = false,
                Tags = tags,
                CreatedAt = txt.CreatedAt,
                UpdatedAt = txt.UpdatedAt,
                SizeBytes = txt.SizeBytes,
                StoragePath = txt.StoragePath,
                Encoding = txt.Encoding
            },
            WordFileTreeItem wrd => new NodeTreeItemDto
            {
                Id = wrd.Id,
                Name = wrd.Name,
                TypeCode = nameof(NodeTypeCode.Word),
                IsDirectory = false,
                Tags = tags,
                CreatedAt = wrd.CreatedAt,
                UpdatedAt = wrd.UpdatedAt,
                SizeBytes = wrd.SizeBytes,
                StoragePath = wrd.StoragePath,
                PageCount = wrd.PageCount
            },
            _ => throw new InvalidOperationException(
                $"Unknown NodeTreeItem type: {item.GetType().Name} for Id: {item.Id}")
        };
    }

    private static TagDto MapTag(Tag tag) => new()
    {
        Id = tag.Id,
        Name = tag.Name,
        Color = tag.Color
    };
}
