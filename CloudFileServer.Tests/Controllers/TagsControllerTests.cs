using CloudFileServer.Controllers;
using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models;
using CloudFileServer.Domain.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CloudFileServer.Tests.Controllers;

public class TagsControllerTests
{
    private readonly Mock<ITagRepository> _repoMock;
    private readonly TagsController       _sut;

    public TagsControllerTests()
    {
        _repoMock = new Mock<ITagRepository>();
        _sut      = new TagsController(_repoMock.Object);
    }

    [Fact]
    public async Task GetAllTags_ReturnsTagDtoList()
    {
        var tags = new List<Tag>
        {
            new() { Id = 1, Name = "Important", Color = "#ff0000" },
            new() { Id = 2, Name = "Archive",   Color = "#aaaaaa" }
        };
        _repoMock.Setup(r => r.GetAllTagsAsync(default)).ReturnsAsync(tags);

        var result = await _sut.GetAllTags(default);

        var ok   = Assert.IsType<OkObjectResult>(result.Result);
        var list = Assert.IsAssignableFrom<IEnumerable<TagDto>>(ok.Value).ToList();
        Assert.Equal(2, list.Count);
        Assert.Equal("Important", list[0].Name);
    }
}
