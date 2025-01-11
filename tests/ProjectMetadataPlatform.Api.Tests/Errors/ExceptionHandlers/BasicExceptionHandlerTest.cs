using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;
using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;
using ProjectMetadataPlatform.Domain.Errors.Interfaces;

namespace ProjectMetadataPlatform.Api.Tests.Errors.ExceptionHandlers;

public class BasicExceptionHandlerTest
{
    private BasicExceptionHandler _basicExceptionHandler;

    [SetUp]
    public void SetUp()
    {
        _basicExceptionHandler = new BasicExceptionHandler();
    }

    [Test]
    public void Handle_EntityNotFoundException_ReturnsNotFound()
    {
        var exception = new EntityNotFoundException("Entity not found");

        var result = _basicExceptionHandler.Handle(exception);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = (NotFoundObjectResult) result;
        Assert.That(notFoundResult.Value, Is.EqualTo(exception.Message));
    }

    [Test]
    public void Handle_DatabaseException_ReturnsInternalServerError()
    {
        var exception = new DatabaseException(new Exception());

        var result = _basicExceptionHandler.Handle(exception);

        Assert.That(result, Is.InstanceOf<StatusCodeResult>());
        var statusCodeResult = (StatusCodeResult) result;
        Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public void Handle_UnknownException_ReturnsInternalServerError()
    {
        var mockException = new Mock<IBasicException>();

        var result = _basicExceptionHandler.Handle(mockException.Object);

        Assert.That(result, Is.InstanceOf<StatusCodeResult>());
        var statusCodeResult = (StatusCodeResult) result;
        Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));
    }
}