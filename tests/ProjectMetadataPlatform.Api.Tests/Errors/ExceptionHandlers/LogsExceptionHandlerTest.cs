using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Api.Tests.Errors.ExceptionHandlers;

public class LogsExceptionHandlerTest
{
    private LogsExceptionHandler _logsExceptionHandler;

    [SetUp]
    public void SetUp()
    {
        _logsExceptionHandler = new LogsExceptionHandler();
    }

    [TestCase (Action.ADDED_PROJECT)]
    [TestCase (Action.ADDED_PROJECT_PLUGIN)]
    [TestCase (Action.UPDATED_PROJECT)]
    [TestCase (Action.UPDATED_PROJECT_PLUGIN)]
    [TestCase (Action.REMOVED_PROJECT_PLUGIN)]
    [TestCase (Action.ARCHIVED_PROJECT)]
    [TestCase (Action.UNARCHIVED_PROJECT)]
    [TestCase (Action.REMOVED_PROJECT)]
    [TestCase (Action.ADDED_GLOBAL_PLUGIN)]
    [TestCase (Action.UPDATED_GLOBAL_PLUGIN)]
    [TestCase (Action.ARCHIVED_GLOBAL_PLUGIN)]
    [TestCase (Action.UNARCHIVED_GLOBAL_PLUGIN)]
    [TestCase (Action.REMOVED_GLOBAL_PLUGIN)]
    public void Handle_LogsNotSupportedAction_ReturnsInternalServerError(Action action)
    {
        var exception = new LogActionNotSupportedException(action);

        var result = _logsExceptionHandler.Handle(exception);

        Assert.That(result, Is.InstanceOf<StatusCodeResult>());
        var statusCodeResult = (StatusCodeResult) result;
        Assert.That(statusCodeResult.StatusCode, Is.EqualTo(500));
    }
}