using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;
using ProjectMetadataPlatform.Domain.Errors.Interfaces;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions that are not use case specific for the Project Metadata Platform API.
/// </summary>
public class BasicExceptionHandler: ControllerBase, IExceptionHandler<IBasicException>
{
    /// <summary>
    /// Handles the specified basic exception and returns an appropriate IActionResult.
    /// </summary>
    /// <param name="exception">The basic exception to Handle.</param>
    /// <returns>An IActionResult representing the result of handling the exception.</returns>
    public IActionResult Handle(IBasicException exception)
    {
        return exception switch
        {
            EntityNotFoundException entityNotFoundException => NotFound(entityNotFoundException.Message),
            _ => throw new ArgumentOutOfRangeException(nameof(exception), exception, null)
        };

    }
}