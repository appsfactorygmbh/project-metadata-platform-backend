using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.UserException;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

public class UserExceptionHandler : ControllerBase, IExceptionHandler<UserException>
{
    public IActionResult? Handle(UserException exception)
    {
        return BadRequest(exception.Message);
    }
}