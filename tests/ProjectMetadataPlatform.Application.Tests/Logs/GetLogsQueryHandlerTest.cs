using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Logs;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Logs;

[TestFixture]
public class GetLogsQueryHandlerTest
{
    private GetLogsQueryHandler _handler;
    private Mock<ILogRepository> _mockLogsRepo;

    [SetUp]
    public void Setup()
    {
        _mockLogsRepo = new Mock<ILogRepository>();
        _handler = new GetLogsQueryHandler(_mockLogsRepo.Object);
    }

    [Test]
    public async Task HandleGetLogs_Test()
    {
        var log = new Log
        {
            Id = 41,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "42",
            Username = "Slartibartfast",
            ProjectId = 43,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange() { Property = "Fjords", OldValue = "None", NewValue = "Many" }
            ]
        };
        var log2 = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "43",
            Username = "Deep Thought",
            ProjectId = 44,
            Project = new Project{ProjectName = "Ultimate Question of Life, the Universe and Everything", ClientName = "Mice", BusinessUnit = "", TeamNumber = 1, Department = ""},
            Action = Action.ARCHIVED_PROJECT,
        };
        var log3 = new Log
        {
            Id = 43,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "44",
            Username = "Infinite Improbability Drive",
            ProjectId = 45,
            Project = new Project{ProjectName = "Atmosphere", ClientName = "", BusinessUnit = "", TeamNumber = 1, Department = ""},
            Action = Action.ADDED_PROJECT_PLUGIN,
            Changes =
            [
                new LogChange() { Property = "flyingObjects", OldValue = "", NewValue = "Wale" }
            ]
        };
        var log4 = new Log
        {
            Id = 44,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "45",
            Username = "Ground",
            ProjectId = 46,
            Project = new Project{ProjectName = "Wale", ClientName = "", BusinessUnit = "", TeamNumber = 1, Department = ""},
            Action = Action.UPDATED_PROJECT_PLUGIN,
            Changes =
            [
                new LogChange() { Property = "alive", OldValue = "yes", NewValue = "no" }
            ]
        };
        var log5 = new Log
        {
            Id = 45,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "46",
            Username = "Prostetnic Vogon Jeltz",
            ProjectId = 47,
            Project = new Project{ProjectName = "Solarsystem", ClientName = "Mice", BusinessUnit = "", TeamNumber = 1, Department = ""},
            Action = Action.REMOVED_PROJECT_PLUGIN,
            Changes =
            [
                new LogChange() { Property = "Earth", OldValue = "intact", NewValue = "destroyed" }
            ]
        };

        _mockLogsRepo.Setup(m => m.GetAllLogs()).ReturnsAsync([log, log2, log3, log4, log5]);

        var request = new GetLogsQuery();
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<LogResponse>>());
        Assert.That(result, Has.Count.EqualTo(5));

        LogResponse logResponse = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Slartibartfast updated project properties:  set Fjords from None to Many"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });

        logResponse = result[1];
        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Deep Thought archived project Ultimate Question of Life, the Universe and Everything"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });

        logResponse = result[2];
        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Infinite Improbability Drive added a new plugin to project Atmosphere with properties: flyingObjects = Wale"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });

        logResponse = result[3];
        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Ground updated plugin properties in project Wale:  set alive from yes to no"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });

        logResponse = result[4];
        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Prostetnic Vogon Jeltz removed a plugin from project Solarsystem with properties: Earth = destroyed"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });

        _mockLogsRepo.Verify(m => m.GetAllLogs(), Times.Once);
    }

    [Test]
    public async Task HandleGetLogs_ForProject_Test()
    {
        _mockLogsRepo.Setup(m => m.GetLogsForProject(It.IsAny<int>())).ReturnsAsync([]);

        var request = new GetLogsQuery(42);
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<LogResponse>>());
        Assert.That(result, Has.Count.EqualTo(0));

        _mockLogsRepo.Verify(m => m.GetLogsForProject(42), Times.Once);
    }

    [Test]
    public async Task HandleGetLogs_ApplySearch_Test()
    {
        var log = new Log
        {
            Id = 41,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "42",
            Username = "Slartibartfast",
            ProjectId = 43,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange() { Property = "Fjords", OldValue = "None", NewValue = "Many" }
            ]
        };

        var log2 = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            UserId = "43",
            Username = "Zaphod Beeblebrox",
            ProjectId = 44,
            Action = Action.ADDED_PROJECT,
            Changes =
            [
                new LogChange() { Property = "ProjectName", OldValue = "", NewValue = "Heart Of Gold" },
                new LogChange() { Property = "InfiniteImprobabilityDrive", OldValue = "", NewValue = "Yes" }
            ]
        };

        _mockLogsRepo.Setup(m => m.GetAllLogs()).ReturnsAsync([log, log2]);

        var request = new GetLogsQuery(null, "gold");
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<LogResponse>>());
        Assert.That(result, Has.Count.EqualTo(1));

        LogResponse logResponse = result.First();
        Assert.Multiple(() =>
        {
            Assert.That(logResponse.LogMessage, Is.EqualTo("Zaphod Beeblebrox created a new project with properties: ProjectName = Heart Of Gold, InfiniteImprobabilityDrive = Yes"));
            Assert.That(logResponse.Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });
    }
}
