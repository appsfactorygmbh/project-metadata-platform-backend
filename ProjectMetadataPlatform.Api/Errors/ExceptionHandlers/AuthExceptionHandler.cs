using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

public class AuthExceptionHandler: ControllerBase, IExceptionHandler<AuthException>
{
    public IActionResult? Handle(AuthException exception)
    {
        return exception switch
        {
            AuthInvalidLoginCredentialsException authInvalidLoginCredentialsException => BadRequest(authInvalidLoginCredentialsException.Message),
            AuthInvalidRefreshTokenException authInvalidRefreshTokenException => BadRequest(authInvalidRefreshTokenException.Message),
            _ => null
        };
    }
}