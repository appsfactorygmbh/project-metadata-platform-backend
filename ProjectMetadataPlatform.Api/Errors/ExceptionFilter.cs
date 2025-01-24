using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;

namespace ProjectMetadataPlatform.Api.Errors;

/// <summary>
/// A filter that handles exceptions in the Project Metadata Platform API.
/// </summary>
public class ExceptionFilter: IExceptionFilter
{
    /// <summary>
    /// Handler for basic exceptions.
    /// </summary>
    private readonly IExceptionHandler<PmpException> _basicExceptionHandler;
    private readonly IExceptionHandler<LogException> _logExceptionHandler;
    private readonly IExceptionHandler<ProjectException> _projectExceptionHandler;
    private readonly IExceptionHandler<PluginException> _pluginExceptionHandler;
    private readonly IExceptionHandler<AuthException> _authExceptionHandler;

/// <summary>
/// Initializes a new instance of the <see cref="ExceptionFilter"/> class.
/// </summary>
/// <param name="basicExceptionHandler">The handler for basic exceptions.</param>
/// <param name="logExceptionHandler">The handler for log exceptions.></param>
/// <param name="projectExceptionHandler">The handler for project exceptions.</param>
/// <param name="pluginExceptionHandler">The handler for global plugin exceptions.</param>
/// <param name="authExceptionHandler">The handler for authentication exceptions.</param>
    public ExceptionFilter(
        IExceptionHandler<PmpException> basicExceptionHandler,
        IExceptionHandler<ProjectException> projectExceptionHandler,
        IExceptionHandler<LogException> logExceptionHandler,
        IExceptionHandler<PluginException> pluginExceptionHandler,
        IExceptionHandler<AuthException> authExceptionHandler)
{
    _basicExceptionHandler = basicExceptionHandler;
    _projectExceptionHandler = projectExceptionHandler;
        _pluginExceptionHandler = pluginExceptionHandler;
        _logExceptionHandler = logExceptionHandler;
    _authExceptionHandler = authExceptionHandler;
}
    /// <summary>
    /// Called when an exception occurs during the execution of an action.
    /// Builds an appropriate http response based on the exception.
    /// </summary>
    /// <param name="context">The context in which the exception occurred.</param>
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        var response = exception switch
        {
            ProjectException projectEx => _projectExceptionHandler.Handle(projectEx),
            PluginException pluginEx => _pluginExceptionHandler.Handle(pluginEx),
            LogException logEx => _logExceptionHandler.Handle(logEx),
            AuthException authEx => _authExceptionHandler.Handle(authEx),
            PmpException basicEx => _basicExceptionHandler.Handle(basicEx),
            _ => HandleUnknownError(exception)
        };

        context.Result = response ?? HandleUnknownError(exception);
    }

    /// <summary>
    /// Handles unknown errors and returns a 500 Internal Server Error status.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the unknown error.</returns>
    private static StatusCodeResult HandleUnknownError(Exception exception)
    {
        Console.WriteLine(exception.Message);
        Console.WriteLine(exception.StackTrace);
        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}