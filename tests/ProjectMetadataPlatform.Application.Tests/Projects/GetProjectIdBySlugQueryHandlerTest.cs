using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

public class GetProjectIdBySlugQueryHandlerTest
{
    private Mock<ISlugHelper> _slugHelperMock;
    private GetProjectIdBySlugQueryHandler _handler;

    [SetUp]
    public void Setup()
    {
        _slugHelperMock = new Mock<ISlugHelper>();
        _handler = new GetProjectIdBySlugQueryHandler(_slugHelperMock.Object);
    }

    [Test]
    public async Task HandleReturnsId_Test()
    {
        var query = new GetProjectIdBySlugQuery("slug");
        _slugHelperMock.Setup(x => x.GetProjectIdBySlug("slug")).ReturnsAsync(1);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.That(result, Is.EqualTo(1));
        _slugHelperMock.Verify(x => x.GetProjectIdBySlug("slug"), Times.Once);
    }

    [Test]
    public void HandleThrowsException_Test()
    {
        var query = new GetProjectIdBySlugQuery("slug");
        _slugHelperMock.Setup(x => x.GetProjectIdBySlug("slug")).ThrowsAsync(new InvalidOperationException("Project with this slug does not exist: slug"));

        Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(query, CancellationToken.None));
        _slugHelperMock.Verify(x => x.GetProjectIdBySlug("slug"), Times.Once);
    }
}