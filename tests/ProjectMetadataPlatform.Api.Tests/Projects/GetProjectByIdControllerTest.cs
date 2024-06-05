using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Projects;
using ProjectMetadataPlatform.Api.Projects.Models;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;


namespace ProjectMetadataPlatform.Api.Tests.Projects;

[TestFixture]
public class GetProjectByIdControllerTest
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

    public async Task GetProjectbyId_NonexistentProject_Test()
    {
        _mediator.Setup(m => m.Send(It.Is<GetProjectQuery>(q => q.Id == 1), It.IsAny<CancellationToken>())).ReturnsAsync((Project) null);
        var result = await _controller.Get(1) ;
        Assert.IsNotNull(result);
        
        Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
        
        
        
    }
    
    [Test]
    public async Task GetProjectTest()
    {
        // prepare
        var projectsResponseContent = new Project
        {
            
            Id= 50,
            ProjectName= "MetaDataPlatform", 
            ClientName="Appsfactory", 
            BusinessUnit="BusinessUnit", 
            TeamNumber=200, 
            Department="Security"
            
        };
        _mediator.Setup(m => m.Send(It.Is<GetProjectQuery>(q => q.Id == 50), It.IsAny<CancellationToken>())).ReturnsAsync(projectsResponseContent);
        
        // act
        var result = await _controller.Get(50);
        
        // assert
        Assert.IsInstanceOf<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsInstanceOf<GetProjectResponse>(okResult.Value);

        var getProjectsResponse = okResult.Value as GetProjectResponse;
        Assert.IsNotNull(getProjectsResponse);




        var project = getProjectsResponse;
        Assert.That(project.ProjectName, Is.EqualTo("MetaDataPlatform"));
        Assert.That(project.ClientName, Is.EqualTo("Appsfactory"));
        Assert.That(project.BusinessUnit, Is.EqualTo("BusinessUnit"));
        Assert.That(project.TeamNumber, Is.EqualTo(200));
        Assert.That(project.Department, Is.EqualTo("Security"));
    }
}