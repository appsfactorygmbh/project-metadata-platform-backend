using System;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

public class PluginsExceptionHandler : ControllerBase, IExceptionHandler<PluginException>
{
    public IActionResult Handle(PluginException exception)
    {
        return exception switch
        {
            PluginNotArchivedException pluginNotArchivedException => BadRequest(pluginNotArchivedException.Message),
            _ => HandleUnknownException(exception)
        };
    }

    private StatusCodeResult HandleUnknownException(PluginException exception)
    {
        Console.WriteLine(exception.Message);
        Console.WriteLine(exception.StackTrace);
        return StatusCode(500);
    }

}