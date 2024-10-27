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
    [HttpPut("{userId:int}")]
    public async Task<ActionResult> Put(int userId,[FromBody] CreateUserRequest request)
    {
        if (userId==0 || string.IsNullOrWhiteSpace(request.Name)
                                                           || string.IsNullOrWhiteSpace(request.Username)
                                                           || string.IsNullOrWhiteSpace(request.Email)
                                                           || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("userId, name, username, email and password must not be empty.");
        }

        var command = new CreateUserCommand(userId,request.Username, request.Name, request.Email, request.Password);

        try
        {
            _ = await _mediator.Send(command);
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


        return new StatusCodeResult(StatusCodes.Status201Created);
    }
}
