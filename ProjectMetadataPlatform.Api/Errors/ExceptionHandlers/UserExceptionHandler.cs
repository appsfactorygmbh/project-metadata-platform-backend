using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.UserException;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions of type <see cref="UserException"/>.
/// </summary>
public class UserExceptionHandler : ControllerBase, IExceptionHandler<UserException>
{
    /// <summary>
    /// Handles the specified <see cref="UserException"/>.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the exception handling.</returns>
    public IActionResult? Handle(UserException exception)
    {
        return exception switch
        {
            UserInvalidPasswordFormatException _ => BadRequest(exception.Message),
            UserUnauthorizedException _ => Unauthorized(exception.Message),
            UserCantDeleteThemselfException => BadRequest(exception.Message),
            _ => null
        };
    }
}