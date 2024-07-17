using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Auth.Models;
using ProjectMetadataPlatform.Application.Auth;


namespace ProjectMetadataPlatform.Api.Auth;

/// <summary>
/// Endpoints for managing authentication.
/// </summary>
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="AuthController"/>.
    /// </summary>
    /// <param name="mediator"></param>
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Logs in a user with the given credentials.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200">Returns the access and refresh tokens.</response>
    /// <response code="400">If the credentials are invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost ("basic")]
    public async Task<ActionResult<LoginResponse>> Post([FromBody] LoginRequest request)
    {
        var query = new LoginQuery(request.Username, request.Password);
        try
        {
            var tokens = await _mediator.Send(query);
            return new LoginResponse(tokens.AccessToken!, tokens.RefreshToken!);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

    }

    /// <summary>
    /// Returns a new access token using the given refresh token.
    /// </summary>
    /// <param name="refreshToken">Refresh Token header in the format 'Refresh refreshToken'</param>
    /// <returns></returns>
    /// <response code="200">Returns the access and refresh tokens.</response>
    /// <response code="400">If the refresh token or the header format are invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet("refresh")]
    public async Task<ActionResult<LoginResponse>> Get([FromHeader(Name = "Authorization")] string refreshToken )
    {

        if (!refreshToken.StartsWith("Refresh "))
        {
            return BadRequest("Invalid Header format");
        }

        var query = new RefreshTokenQuery(refreshToken.Replace("Refresh ", ""));
        try
        {
            var tokens = await _mediator.Send(query);
            return new LoginResponse(tokens.AccessToken!, tokens.RefreshToken!);
        }
        catch (AuthenticationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }


    }

}
