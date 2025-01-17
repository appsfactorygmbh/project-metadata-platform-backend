using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

public class LogsExceptionHandler : ControllerBase, IExceptionHandler<LogException>
{
    public IActionResult Handle(LogException exception)
    {
        Console.WriteLine(exception.Message);
        Console.WriteLine(exception.StackTrace);
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}