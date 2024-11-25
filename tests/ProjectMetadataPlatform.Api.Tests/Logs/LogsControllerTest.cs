using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Api.Logs;
using ProjectMetadataPlatform.Api.Logs.Models;
using ProjectMetadataPlatform.Application.Logs;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.User;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Api.Tests.Logs;

public class LogsControllerTest
{
    private LogsController _controller;
    private Mock<IMediator> _mediator;
    private Mock<ILogConverter> _logConverter;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _logConverter = new Mock<ILogConverter>();
        _controller = new LogsController(_mediator.Object, _logConverter.Object);
    }

    [Test]
    public async Task GetAllLogs_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Slartibartfast",
            Author = new User { Email = "Slartibartfast" },
            ProjectId = 42,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange { Property = "Fjords", OldValue = "None", NewValue = "Many" }
            ]
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetLogsQuery>(), default)).ReturnsAsync([log]);
        _logConverter.Setup(lc => lc.BuildLogMessage(log)).Returns(new LogResponse("Project updated", "1970-01-01T00:00:00+01:00"));

        var result = await _controller.Get(null, null, null, null);

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as IEnumerable<LogResponse>;
        Assert.That(response, Is.Not.Null);
        var logResponses = response.ToList();
        Assert.That(logResponses, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(logResponses[0].LogMessage, Is.EqualTo("Project updated"));
            Assert.That(logResponses[0].Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });

        _mediator.Verify(m => m.Send(It.IsAny<GetLogsQuery>(), default), Times.Once);
        _logConverter.Verify(lc => lc.BuildLogMessage(log), Times.Once);
    }

    [Test]
    public async Task GetLogsForProject_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Slartibartfast",
            Author = new User { Email = "Slartibartfast" },
            ProjectId = 42,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange { Property = "Fjords", OldValue = "None", NewValue = "Many" }
            ]
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetLogsQuery>(), default)).ReturnsAsync([log]);
        _logConverter.Setup(lc => lc.BuildLogMessage(log)).Returns(new LogResponse("Project updated", "1970-01-01T00:00:00+01:00"));

        var result = await _controller.Get(42, null, null, null);

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as IEnumerable<LogResponse>;
        Assert.That(response, Is.Not.Null);
        var logResponses = response.ToList();
        Assert.That(logResponses, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(logResponses[0].LogMessage, Is.EqualTo("Project updated"));
            Assert.That(logResponses[0].Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });

        _mediator.Verify(m => m.Send(It.Is<GetLogsQuery>(q => q.ProjectId == 42), default), Times.Once);
        _logConverter.Verify(lc => lc.BuildLogMessage(log), Times.Once);
    }

    [Test]
    public async Task GetLogsWithSearch_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Slartibartfast",
            Author = new User { Email = "Slartibartfast" },
            ProjectId = 42,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange { Property = "Fjords", OldValue = "None", NewValue = "Many" }
            ]
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetLogsQuery>(), default)).ReturnsAsync([log]);
        _logConverter.Setup(lc => lc.BuildLogMessage(log)).Returns(new LogResponse("Project updated", "1970-01-01T00:00:00+01:00"));

        var result = await _controller.Get(null, "updated", null, null);

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as IEnumerable<LogResponse>;
        Assert.That(response, Is.Not.Null);
        var logResponses = response.ToList();
        Assert.That(logResponses, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(logResponses[0].LogMessage, Is.EqualTo("Project updated"));
            Assert.That(logResponses[0].Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });

        _mediator.Verify(m => m.Send(It.Is<GetLogsQuery>(q => q.Search == "updated"), default), Times.Once);
        _logConverter.Verify(lc => lc.BuildLogMessage(log), Times.Once);
    }

    [Test]
    public async Task GetLogsForAffectedUser_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "Slartibartfast",
            Author = new User { Email = "Slartibartfast" },
            Action = Action.UPDATED_USER,
            AffectedUserId = "Newton",
            Changes =
            [
                new LogChange { Property = "flying", OldValue = "yes", NewValue = "no" }
            ]
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetLogsQuery>(), default)).ReturnsAsync([log]);
        _logConverter.Setup(lc => lc.BuildLogMessage(log)).Returns(new LogResponse("User updated", "1970-01-01T00:00:00+01:00"));

        var result = await _controller.Get(null, null, "Newton", null);

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as IEnumerable<LogResponse>;
        Assert.That(response, Is.Not.Null);
        var logResponses = response.ToList();
        Assert.That(logResponses, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(logResponses[0].LogMessage, Is.EqualTo("User updated"));
            Assert.That(logResponses[0].Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });

        _mediator.Verify(m => m.Send(It.Is<GetLogsQuery>(q => q.UserId == "Newton"), default), Times.Once);
        _logConverter.Verify(lc => lc.BuildLogMessage(log), Times.Once);
    }

    [Test]
    public async Task GetLogsForGlobalPlugin_Test()
    {
        var log = new Log
        {
            Id = 42,
            TimeStamp = new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(1)),
            AuthorId = "42",
            AuthorEmail = "newton",
            Author = new User { Email = "newton" },
            Action = Action.UPDATED_GLOBAL_PLUGIN,
            GlobalPluginId = 42,
            GlobalPluginName = "Gravity",
            Changes =
            [
                new LogChange { Property = "discovered", OldValue = "no", NewValue = "yes" }
            ]
        };
        _mediator.Setup(m => m.Send(It.IsAny<GetLogsQuery>(), default)).ReturnsAsync([log]);
        _logConverter.Setup(lc => lc.BuildLogMessage(log)).Returns(new LogResponse("Global plugin updated", "1970-01-01T00:00:00+01:00"));

        var result = await _controller.Get(null, null, null, 42);

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as IEnumerable<LogResponse>;
        Assert.That(response, Is.Not.Null);
        var logResponses = response.ToList();
        Assert.That(logResponses, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(logResponses[0].LogMessage, Is.EqualTo("Global plugin updated"));
            Assert.That(logResponses[0].Timestamp, Is.EqualTo("1970-01-01T00:00:00+01:00"));
        });

        _mediator.Verify(m => m.Send(It.Is<GetLogsQuery>(q => q.GlobalPluginId == 42), default), Times.Once);
        _logConverter.Verify(lc => lc.BuildLogMessage(log), Times.Once);
    }

    [Test]
    public async Task GetLogs_ProjectNotFound_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetLogsQuery>(), default)).ThrowsAsync(new InvalidOperationException("Something went wrong"));

        var result = await _controller.Get(404, null, null, null);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
        var statusCodeResult = result.Result as NotFoundObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(statusCodeResult?.StatusCode, Is.EqualTo(404));
            Assert.That(statusCodeResult?.Value, Is.Not.Null);
        });
        Assert.That(statusCodeResult.Value, Is.EqualTo("No project with id 404 found"));
    }

    [Test]
    public async Task GetLogs_ThrowsException_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetLogsQuery>(), default)).ThrowsAsync(new FormatException("Something went wrong"));

        var result = await _controller.Get(null, null, null, null);

        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());
        var statusCodeResult = result.Result as StatusCodeResult;
        Assert.That(statusCodeResult?.StatusCode, Is.EqualTo(500));
    }
}
