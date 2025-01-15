using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors;
using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions that are not use case specific for the Project Metadata Platform API.
/// </summary>
public class BasicExceptionHandler: ControllerBase, IExceptionHandler<PmpException>
{
    /// <summary>
    /// Handles the specified basic exception and returns an appropriate IActionResult.
    /// </summary>
    /// <param name="exception">The basic exception to Handle.</param>
    /// <returns>An IActionResult representing the result of handling the exception.</returns>
    public IActionResult Handle(PmpException exception)
    {
        return exception switch
        {
            EntityNotFoundException entityNotFoundException => NotFound(entityNotFoundException.Message),
            DatabaseException databaseException => HandleDatabaseException(databaseException),
            _ => HandleUnknownException(exception)
        };
    }

    /// <summary>
    /// Handles all exceptions that are not caught manually and returns a 500 Internal Server Error status.
    /// </summary>
    /// <param name="exception">The database exception to handle.</param>
    /// <returns>A StatusCodeResult representing the result of handling the database exception.</returns>
    private StatusCodeResult HandleUnknownException(PmpException exception)
    {
        Console.WriteLine(exception.Message);
        Console.WriteLine(exception.StackTrace);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    /// <summary>
    /// Handles database exceptions and returns a 502 status.
    /// </summary>
    /// <param name="databaseException">The database exception to handle.</param>
    /// <returns>A StatusCodeResult representing the result of handling the database exception.</returns>
    private StatusCodeResult HandleDatabaseException(DatabaseException databaseException)
    {
        Console.WriteLine(databaseException.Message);
        Console.WriteLine(databaseException.StackTrace);
        return StatusCode(StatusCodes.Status502BadGateway);
    }
}