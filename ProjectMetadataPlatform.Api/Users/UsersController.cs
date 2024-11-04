using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.User;

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
    private readonly IHttpContextAccessor _httpContextAccessor;
    /// <summary>
    ///     Creates a new instance of the <see cref="UsersController" /> class.
    /// </summary>
    /// <param name="mediator">The mediator instance used for sending commands and queries.</param>
    public UsersController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///    Creates a new user.
    /// </summary>
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
            id = await _mediator.Send(command);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
        var response = new CreateUserResponse(id);
        var uri = "/Users/" + id;
        return Created(uri, response);
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
            return NotFound("No User with id " + userId + " was found.");
        }

        var response = new GetUserResponse(
            user.Id,
            user.UserName ?? "",
            user.Name ?? "",
            user.Email ?? ""
            );
        return Ok(response);
    }

    /// <summary>
    /// Patches the user information.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to be patched.</param>
    /// <param name="request">The request model containing the new user information.</param>
    /// <returns>The updated user information.</returns>
    /// <response code="200">The user was patched successfully.</response>
    /// <response code="404">The user with the specified ID was not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPatch("{userId}")]
    public async Task<ActionResult<GetUserResponse>> Patch(string userId, [FromBody] PatchUserRequest request)
    {
        var command = new PatchUserCommand(userId, request.Username, request.Name, request.Email, request.Password);

        User? user;
        try
        {
            user = await _mediator.Send(command);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);

            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        if (user == null)
        {
            return NotFound("No user with id " + userId + " was found.");
        }

        var response = new GetUserResponse(user.Id, user.UserName ?? "", user.Name, user.Email ?? "");
        return Ok(response);
    }


    /// <summary>
    ///    Gets the current authenticated user's information.
    /// </summary>
    /// <returns>The current user's information.</returns>
    /// <response code="200">The user information is returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    /// <response code="404">The user was not found.</response>
    [HttpGet("Me")]
    public async Task<ActionResult<GetUserResponse>> GetMe()
    {
        var query = new GetUserByUserNameQuery(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value);
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
            return NotFound("User not found.");
        }

        var response = new GetUserResponse(user.Id, user.UserName ?? "", user.Name ?? "", user.Email ?? "");
        return Ok(response);
    }
}
