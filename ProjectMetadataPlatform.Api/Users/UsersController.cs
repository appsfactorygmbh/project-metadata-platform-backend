using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Users;

namespace ProjectMetadataPlatform.Api.Users;

/// <summary>
///    Endpoint for user management.
/// </summary>
[ApiController]
[Authorize]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    /// <summary>
    ///     Creates a new instance of the <see cref="UsersController" /> class.
    /// </summary>
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///    Creates a new user.
    /// </summary>
    /// <param name="userId">Id of the user</param>
    /// <param name="request">Request containing user information</param>
    /// <returns>Statuscode representing the result of user creation</returns>
    /// <response code="201">The user was created successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    /// <response code="400">The request was invalid.</response>
    [HttpPut]
    public async Task<ActionResult<CreateUserResponse>> Put([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name)
            || string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("name, username, email and password must not be empty.");
        }

        var command = new CreateUserCommand(request.Username, request.Name, request.Email, request.Password);
        string id;
        try
        {
            id= await _mediator.Send(command);
        }
        catch ( ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        var response = new CreateUserResponse(int.Parse(id));
        var uri = "/Users/" + id;
        return Created(uri,response);
    }
}
