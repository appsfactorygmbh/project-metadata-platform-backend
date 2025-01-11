using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;
using ProjectMetadataPlatform.Domain.Errors.Interfaces;
using RouteData = Microsoft.AspNetCore.Routing.RouteData;

namespace ProjectMetadataPlatform.Api.Tests.Errors;

public class ExceptionFilterTest
{
    private ExceptionFilter _filter;
    private Mock<IExceptionHandler<IBasicException>> _basicExceptionHandler;
    private Mock<ExceptionContext> _context;

    [SetUp]
    public void SetUp()
    {
        _basicExceptionHandler = new Mock<IExceptionHandler<IBasicException>>();
        _context = SetupExceptionContext();
        _filter = new ExceptionFilter(_basicExceptionHandler.Object);
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
        _basicExceptionHandler.Setup(h => h.Handle(It.IsAny<IBasicException>())).Returns(result);

        _context.SetupSet(c => c.Result = It.IsAny<IActionResult>()).Callback((IActionResult r) =>
        {
            Assert.That(r, Is.EqualTo(result));
        });

        _filter.OnException(_context.Object);

        _basicExceptionHandler.Verify(h => h.Handle(It.IsAny<IBasicException>()), Times.Once);
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

        _basicExceptionHandler.Verify(h => h.Handle(It.IsAny<IBasicException>()), Times.Never);
        _context.VerifySet(c => c.Result = It.IsAny<IActionResult>(), Times.Once);
    }
}