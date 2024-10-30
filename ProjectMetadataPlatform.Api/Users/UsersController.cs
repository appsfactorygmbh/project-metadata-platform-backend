using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.User;
using ProjectMetadataPlatform.Api.Users.Models;

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
    ///     Gets all users.
    /// </summary>
    /// <returns>All users.</returns>
    /// <response code="200">The users are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetAllUsersResponse>>> Get()
    {
        var query = new GetAllUsersQuery();
        IEnumerable<User> users;
        try
        {
            users = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        IEnumerable<GetAllUsersResponse> response = users.Select(user => new GetAllUsersResponse(
            user.Id,
            user.Name));
        return Ok(response);
    }

        /// <summary>
    ///     Gets a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>The user with the specified ID.</returns>
    /// <response code="200">The user is returned successfully.</response>
    /// <response code="404">The user with the specified ID was not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{userId}")]
    public async Task<ActionResult<GetUserResponse>> GetUserById(string userId)
    {
        var query = new GetUserQuery(userId);
        User? user;
        try
        {
            user = await _mediator.Send(query);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        if (user == null)
        {
            return NotFound();
        }

        var response = new GetUserResponse(
            user.Id,
            user.UserName ?? "",
            user.Name,
            user.Email ?? ""
            );
        return Ok(response);
    }
}
