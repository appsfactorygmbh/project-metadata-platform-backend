using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ProjectMetadataPlatform.Api.Projects;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Api.Tests.Projects;

[TestFixture]
public class GetProjectsBySearchControllerTest
{
    private ProjectsController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new ProjectsController(_mediator.Object);
    }



    [Test]
    public async Task GetProjectTest()
    {
        // prepare
        var projectsResponseContent = new Project
        {
            Id = 50,
            ProjectName = "MetaDataPlatform",
            ClientName = "Appsfactory",
            BusinessUnit = "BusinessUnit",
            TeamNumber = 200,
            Department = "Security"
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetProjectQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);

        // act
        var result = await _controller.Get("M");

        // assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOf<GetProjectResponse>(okResult.Value);

        var getProjectsResponse = okResult.Value as GetProjectResponse;
        Assert.IsNotNull(getProjectsResponse);

        
    }
}
