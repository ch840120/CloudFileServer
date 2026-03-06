using CloudFileServer.Controllers;
using CloudFileServer.Domain.Interfaces;
using CloudFileServer.Domain.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace CloudFileServer.Tests.Controllers;

public class NodesControllerTests
{
    private readonly Mock<INodeEditRepository> _repoMock;
    private readonly Mock<IFileStorageService> _storageMock;
    private readonly Mock<ITagRepository>      _tagRepoMock;
    private readonly Mock<IMemoryCache>        _cacheMock;
    private readonly NodesController           _sut;

    public NodesControllerTests()
    {
        _repoMock    = new Mock<INodeEditRepository>();
        _storageMock = new Mock<IFileStorageService>();
        _tagRepoMock = new Mock<ITagRepository>();
        _cacheMock   = new Mock<IMemoryCache>();

        object? cacheEntry = null;
        _cacheMock.Setup(c => c.TryGetValue(It.IsAny<object>(), out cacheEntry)).Returns(false);
        _cacheMock.Setup(c => c.Remove(It.IsAny<object>()));

        _sut = new NodesController(
            _repoMock.Object,
            _storageMock.Object,
            _tagRepoMock.Object,
            _cacheMock.Object);
    }

    [Fact]
    public async Task SoftDelete_ValidId_Returns204AndInvalidatesCache()
    {
        _repoMock.Setup(r => r.SoftDeleteAsync(42, default)).Returns(Task.CompletedTask);

        var result = await _sut.SoftDelete(42, default);

        Assert.IsType<NoContentResult>(result);
        _repoMock.Verify(r => r.SoftDeleteAsync(42, default), Times.Once);
        _cacheMock.Verify(c => c.Remove("file-tree"), Times.Once);
    }

    [Fact]
    public async Task Copy_ValidRequest_ReturnsNewNodeIdAndCopiesFiles()
    {
        var fileMappings = new List<(string, string)> { ("/uploads/a.txt", "/uploads/a_copy_1.txt") };
        var copyResult   = new CopyResult(99, fileMappings.AsReadOnly());
        _repoMock
            .Setup(r => r.CopySubtreeAsync(1, null, default))
            .ReturnsAsync(copyResult);

        var result = await _sut.Copy(1, new CopyNodeRequest { TargetParentId = null }, default);

        var ok       = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<NodeOperationResponse>(ok.Value);
        Assert.Equal(99, response.NewNodeId);
        _storageMock.Verify(s => s.CopyFile("/uploads/a.txt", "/uploads/a_copy_1.txt"), Times.Once);
        _cacheMock.Verify(c => c.Remove("file-tree"), Times.Once);
    }

    [Fact]
    public async Task Patch_WithIsDeletedFalse_CallsRestoreAsyncAndInvalidatesCache()
    {
        _repoMock.Setup(r => r.RestoreAsync(7, default)).Returns(Task.CompletedTask);

        var result = await _sut.Patch(7, new PatchNodeRequest { IsDeleted = false }, default);

        Assert.IsType<NoContentResult>(result);
        _repoMock.Verify(r => r.RestoreAsync(7, default), Times.Once);
        _cacheMock.Verify(c => c.Remove("file-tree"), Times.Once);
    }

    [Fact]
    public async Task AddTag_ValidRequest_Returns204AndInvalidatesCache()
    {
        _tagRepoMock.Setup(r => r.AddTagToNodeAsync(10, 2, default)).Returns(Task.CompletedTask);

        var result = await _sut.AddTag(10, new AddTagRequest { TagId = 2 }, default);

        Assert.IsType<NoContentResult>(result);
        _tagRepoMock.Verify(r => r.AddTagToNodeAsync(10, 2, default), Times.Once);
        _cacheMock.Verify(c => c.Remove("file-tree"), Times.Once);
    }

    [Fact]
    public async Task RemoveTag_ValidIds_Returns204AndInvalidatesCache()
    {
        _tagRepoMock.Setup(r => r.RemoveTagFromNodeAsync(10, 2, default)).Returns(Task.CompletedTask);

        var result = await _sut.RemoveTag(10, 2, default);

        Assert.IsType<NoContentResult>(result);
        _tagRepoMock.Verify(r => r.RemoveTagFromNodeAsync(10, 2, default), Times.Once);
        _cacheMock.Verify(c => c.Remove("file-tree"), Times.Once);
    }
}
