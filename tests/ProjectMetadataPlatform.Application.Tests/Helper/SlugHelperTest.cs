using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Helper;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Helper;

public class SlugHelperTest
{
    private SlugHelper _slugHelper;
    private Mock<IProjectsRepository> _mockProjectsRepository;

    [SetUp]
    public void Setup()
    {
        _mockProjectsRepository = new Mock<IProjectsRepository>();
        _slugHelper = new SlugHelper(_mockProjectsRepository.Object);
    }

    [Test]
    public void GenerateSlug_Test()
    {
        const string input = "Example Project Version 3-14-1;";
        const string expected = "example_project_version_3141";

        var result = _slugHelper.GenerateSlug(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GenerateSlug_Test_ToLowerCase()
    {
        const string input = "ExampleProject";
        const string expected = "exampleproject";

        var result = _slugHelper.GenerateSlug(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GenerateSlug_Test_ReplaceWhitespaces()
    {
        const string input = "Example Project";
        const string expected = "example_project";

        var result = _slugHelper.GenerateSlug(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GenerateSlug_Test_TrimWhitespaces()
    {
        const string input = " ExampleProject ";
        const string expected = "exampleproject";

        var result = _slugHelper.GenerateSlug(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void GenerateSlug_Test_ReplaceSpecialChars()
    {
        const string input = "ExampleProject!§$%&/();:-";
        const string expected = "exampleproject";

        var result = _slugHelper.GenerateSlug(input);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetProjectIdBySlug_Test()
    {
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "",
            BusinessUnit = "",
            Department = "",
            TeamNumber = 0
        };

        const string slug = "example_project";
        const int expected = 1;

        _mockProjectsRepository.Setup(m => m.GetProjectBySlugAsync(It.IsAny<string>())).ReturnsAsync(exampleProject);

        var result = await _slugHelper.GetProjectIdBySlug(slug);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetProjectIdBySlug_Test_ThrowsException()
    {
        const string slug = "example_project";

        _mockProjectsRepository.Setup(m => m.GetProjectBySlugAsync(It.IsAny<string>())).ReturnsAsync((Project)null!);

        Assert.ThrowsAsync<InvalidOperationException>(() => _slugHelper.GetProjectIdBySlug(slug));
    }
}