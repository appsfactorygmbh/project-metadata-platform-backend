using System.Collections.Generic;
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
    private  IMediator _mediator;

    [SetUp]
    public void Setup(IMediator mediator)
    {
        _mediator = mediator;
        _controller = new ProjectsController(_mediator);
    }



    [Test]
    public async Task GetProjectTest()
    {
        // prepare
        var projectsResponseContent = new List<Project>()
        {
            new Project
            {
                Id = 50,
                ProjectName = "MetaDataPlatform",
                ClientName = "Appsfactory",
                BusinessUnit = "BusinessUnit",
                TeamNumber = 200,
                Department = "Security"
            },
            new Project
            {
                Id = 50,
                ProjectName = "DataPlatform",
                ClientName = "Appsfactory",
                BusinessUnit = "BusinessUnit",
                TeamNumber = 200,
                Department = "Security"
            }
        };
         
        /*_mediator.Setup(m => m.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);*/
        await _mediator.Send(It.IsAny<GetAllProjectsQuery>(), It.IsAny<CancellationToken>());
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
