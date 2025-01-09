using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.Interfaces;

namespace ProjectMetadataPlatform.Api.Errors;

/// <summary>
/// A filter that handles exceptions in the Project Metadata Platform API.
/// </summary>
public class ExceptionFilter: IExceptionFilter
{
    /// <summary>
    /// Handler for basic exceptions.
    /// </summary>
    private readonly IExceptionHandler<IBasicException> _basicExceptionHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionFilter"/> class.
    /// </summary>
    /// <param name="basicExceptionHandler">The handler for basic exceptions.</param>
    public ExceptionFilter(IExceptionHandler<IBasicException> basicExceptionHandler)
    {
        _basicExceptionHandler = basicExceptionHandler;
    }

    /// <summary>
    /// Called when an exception occurs during the execution of an action.
    /// Builds an appropriate http response based on the exception.
    /// </summary>
    /// <param name="context">The context in which the exception occurred.</param>
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        context.Result = exception switch
        {
            IBasicException basicEx => _basicExceptionHandler.Handle(basicEx),
            _ => HandleUnknownError(exception)
        };
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