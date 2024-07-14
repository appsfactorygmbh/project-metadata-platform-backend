using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [HttpGet("refresh")]

    public async Task<ActionResult<LoginResponse>> Get([FromHeader(Name = "Authorization")] RefreshRequest request )
    {

        var query = new RefreshTokenQuery(request.AccessToken, request.RefreshToken);
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

}
