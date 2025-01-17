using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors;
using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;
using RouteData = Microsoft.AspNetCore.Routing.RouteData;

namespace ProjectMetadataPlatform.Api.Tests.Errors;

public class ExceptionFilterTest
{
    private ExceptionFilter _filter;
    private Mock<IExceptionHandler<PmpException>> _basicExceptionHandler;
    private Mock<IExceptionHandler<LogException>> _logExceptionHandler;
    private Mock<IExceptionHandler<ProjectException>> _projectExceptionHandler;
    private Mock<ExceptionContext> _context;

    [SetUp]
    public void SetUp()
    {
        _basicExceptionHandler = new Mock<IExceptionHandler<PmpException>>();
        _projectExceptionHandler = new Mock<IExceptionHandler<ProjectException>>();
        _logExceptionHandler = new Mock<IExceptionHandler<LogException>>();
        _context = SetupExceptionContext();
        _filter = new ExceptionFilter(_basicExceptionHandler.Object, _projectExceptionHandler.Object, _logExceptionHandler.Object);
    }

    private static Mock<ExceptionContext> SetupExceptionContext()
    {
        var mockHttpContext = new Mock<HttpContext>();
        var mockRouteData = new RouteData();
        var mockActionDescriptor = new ActionDescriptor();
        var mockActionContext = new Mock<ActionContext>(mockHttpContext.Object, mockRouteData, mockActionDescriptor);
        return new Mock<ExceptionContext>(mockActionContext.Object, new List<IFilterMetadata>());
    }

    [Test]
    public void CallsBasicExceptionHandlerForBasicException_Test()
    {
        var mockException = new Mock<EntityNotFoundException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        var result = new StatusCodeResult(404);
        _basicExceptionHandler.Setup(h => h.Handle(It.IsAny<PmpException>())).Returns(result);

        _context.SetupSet(c => c.Result = It.IsAny<IActionResult>()).Callback((IActionResult r) =>
        {
            Assert.That(r, Is.EqualTo(result));
        });

        _filter.OnException(_context.Object);

        _basicExceptionHandler.Verify(h => h.Handle(It.IsAny<PmpException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void CallsProjectsExceptionHandlerForProjectException_Test()
    {
        var mockException = new Mock<ProjectException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        var result = new StatusCodeResult(500);
        _projectExceptionHandler.Setup(h => h.Handle(It.IsAny<ProjectException>())).Returns(result);

        _context.SetupSet(c => c.Result = It.IsAny<IActionResult>()).Callback((IActionResult r) =>
        {
            Assert.That(r, Is.EqualTo(result));
        });

        _filter.OnException(_context.Object);

        _projectExceptionHandler.Verify(h => h.Handle(It.IsAny<ProjectException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void HandlesUnknownException_Test()
    {
        var mockException = new Mock<Exception>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        _context.SetupSet(c => c.Result = It.IsAny<IActionResult>()).Callback((IActionResult r) =>
        {
            Assert.That(r, Is.InstanceOf<StatusCodeResult>());
            var statusCodeResult = (StatusCodeResult) r;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        });

        _filter.OnException(_context.Object);

        _basicExceptionHandler.Verify(h => h.Handle(It.IsAny<PmpException>()), Times.Never);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void CallLogsExceptionHandlerForLogException_Test()
    {
        var mockException = new Mock<LogException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        var result = new StatusCodeResult(500);
        _logExceptionHandler.Setup(h => h.Handle(It.IsAny<LogException>())).Returns(result);

        _context.SetupSet(c => c.Result = It.IsAny<IActionResult>()).Callback((IActionResult r) =>
        {
            Assert.That(r, Is.EqualTo(result));
        });

        _filter.OnException(_context.Object);

        _logExceptionHandler.Verify(h => h.Handle(It.IsAny<LogException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void HandlesNullReturnOfExceptionHandler_Test()
    {
        var mockException = new Mock<ProjectException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        _context.SetupSet(c => c.Result = It.IsAny<IActionResult>()).Callback((IActionResult r) =>
        {
            Assert.That(r, Is.InstanceOf<StatusCodeResult>());
            var statusCodeResult = (StatusCodeResult) r;
            Assert.That(statusCodeResult.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        });
        _projectExceptionHandler.Setup(h => h.Handle(It.IsAny<ProjectException>())).Returns((IActionResult?)null);

        _filter.OnException(_context.Object);

        _projectExceptionHandler.Verify(h => h.Handle(It.IsAny<ProjectException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }

    [Test]
    public void CallLogsExceptionHandlerForProjectException_Test()
    {
        var mockException = new Mock<LogException>("some error message");
        _context.SetupGet(c => c.Exception).Returns(mockException.Object);

        var result = new StatusCodeResult(500);
        _logExceptionHandler.Setup(h => h.Handle(It.IsAny<LogException>())).Returns(result);

        _context.SetupSet(c => c.Result = It.IsAny<IActionResult>()).Callback((IActionResult r) =>
        {
            Assert.That(r, Is.EqualTo(result));
        });

        _filter.OnException(_context.Object);

        _logExceptionHandler.Verify(h => h.Handle(It.IsAny<LogException>()), Times.Once);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }
}