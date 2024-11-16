using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Logs;
using ProjectMetadataPlatform.Application.Logs;

namespace ProjectMetadataPlatform.Api.Tests.Logs;

public class LogsControllerTest
{
    private LogsController _controller;
    private Mock<IMediator> _mediator;

    [SetUp]
    public void Setup()
    {
        _mediator = new Mock<IMediator>();
        _controller = new LogsController(_mediator.Object);
    }

    [Test]
    public async Task GetAllLogs_Test()
    {
        var dummyLog = new List<LogResponse>(
            [
                new LogResponse("something happened and we know it", "1970-01-01T00:00:00+00:00")
            ]
        );

        _mediator.Setup(m => m.Send(It.IsAny<GetLogsQuery>(), default)).ReturnsAsync(dummyLog);

        var result = await _controller.Get(null, null);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.Value, Is.InstanceOf<List<LogResponse>>());

        var logs = okResult?.Value as List<LogResponse>;
        Assert.That(logs, Is.Not.Null);
        Assert.That(logs, Has.Count.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(logs[0].LogMessage, Is.EqualTo("something happened and we know it"));
            Assert.That(logs[0].Timestamp, Is.EqualTo("1970-01-01T00:00:00+00:00"));
        });

        _mediator.Verify(m => m.Send(It.Is<GetLogsQuery>(q => q.ProjectId == null && q.Search == null), default));
    }

    [Test]
    public async Task GetLogsForProject_Test()
    {
        var projectId = 42;

        _mediator.Setup(m => m.Send(It.IsAny<GetLogsQuery>(), default)).ReturnsAsync(new List<LogResponse>());

        var result = await _controller.Get(projectId, null);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.Value, Is.InstanceOf<List<LogResponse>>());

        var logs = okResult?.Value as List<LogResponse>;
        Assert.That(logs, Is.Not.Null);
        Assert.That(logs, Has.Count.EqualTo(0));
        _mediator.Verify(m => m.Send(It.Is<GetLogsQuery>(q => q.ProjectId == projectId && q.Search == null), default));
    }

    [Test]
    public async Task GetLogsWithSearch_Test()
    {
        var query = "euler's number";

        _mediator.Setup(m => m.Send(It.IsAny<GetLogsQuery>(), default)).ReturnsAsync(new List<LogResponse>());

        var result = await _controller.Get(null, query);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.Value, Is.InstanceOf<List<LogResponse>>());

        var logs = okResult?.Value as List<LogResponse>;
        Assert.That(logs, Is.Not.Null);
        Assert.That(logs, Has.Count.EqualTo(0));
        _mediator.Verify(m => m.Send(It.Is<GetLogsQuery>(q => q.ProjectId == null && q.Search == query), default));
    }

    [Test]
    public async Task GetLogs_ProjectNotFound_Test()
    {
        _mediator.Setup(m => m.Send(It.IsAny<GetLogsQuery>(), default)).ThrowsAsync(new InvalidOperationException("Something went wrong"));

        var result = await _controller.Get(404, null);

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

        var result = await _controller.Get(null, null);

        Assert.That(result.Result, Is.InstanceOf<StatusCodeResult>());
        var statusCodeResult = result.Result as StatusCodeResult;
        Assert.That(statusCodeResult?.StatusCode, Is.EqualTo(500));
    }
}
