using System.Collections.Generic;
using System.Linq;
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
    private  Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
         _mediator = new Mock<IMediator>();
        _controller = new ProjectsController(_mediator.Object);
    }


    [Test]
    //[Ignore("not finished prolly will not able to be")]
    public async Task GetProjectTest()
    {
        // prepare
        var projectsResponseContent = new List<Project>()
        {
            new()
            {
                Id = 0,
                ProjectName = "Regen",
                ClientName = "Nasa",
                BusinessUnit = "BuWeather",
                TeamNumber = 42,
                Department = "Homelandsecurity"
            }
            
        };
         
        _mediator.Setup(m => m.Send(It.Is<GetAllProjectsQuery>(x => x.Search == "M"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectsResponseContent);
        
        // act
        var result = await _controller.Get("M");

        // assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOf<IEnumerable<GetProjectsResponse>>(okResult.Value);

        var getProjectsResponseEnumeration = okResult.Value as IEnumerable<GetProjectsResponse>;
        Assert.IsNotNull(getProjectsResponseEnumeration);
        
        var getProjectsResponseArray = getProjectsResponseEnumeration as GetProjectsResponse[] ?? getProjectsResponseEnumeration.ToArray();
        Assert.That(getProjectsResponseArray, Has.Length.EqualTo(1));

        var project = getProjectsResponseArray.First();
        Assert.That(project.ProjectName, Is.EqualTo("Regen"));
        Assert.That(project.ClientName, Is.EqualTo("Nasa"));
        Assert.That(project.BusinessUnit, Is.EqualTo("BuWeather"));
        Assert.That(project.TeamNumber, Is.EqualTo(42));
        
    }
}
