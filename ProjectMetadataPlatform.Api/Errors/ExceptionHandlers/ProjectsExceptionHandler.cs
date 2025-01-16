using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to projects in the Project Metadata Platform API.
/// </summary>
public class ProjectsExceptionHandler: ControllerBase, IExceptionHandler<ProjectException>
{
    /// <summary>
    /// Handles a specific project exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The project exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the project exception.</returns>
    public IActionResult Handle(ProjectException exception)
    {
        return exception switch
        {
            ProjectNotArchivedException projectNotArchivedException => BadRequest(projectNotArchivedException.Message),
            _ => HandleUnknownException(exception)
        };
    }

    /// <summary>
    /// Handles all exceptions that are not caught manually and returns a 500 Internal Server Error status.
    /// </summary>
    /// <param name="exception">The project exception to handle.</param>
    /// <returns>A StatusCodeResult representing the result of handling the project exception.</returns>
    private StatusCodeResult HandleUnknownException(ProjectException exception)
    {
        Console.WriteLine(exception.Message);
        Console.WriteLine(exception.StackTrace);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}