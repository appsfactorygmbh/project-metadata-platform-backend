using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Plugins.Models;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Users;

namespace ProjectMetadataPlatform.Api.Users;

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

    [HttpPut("{UserId:int}")]
    public async Task<ActionResult<StatusCodeResult>> Put(int UserId,[FromBody] CreateUserRequest request)
    {


        var command = new CreateUserCommand(UserId,request.Username, request.Name, request.Email, request.Password);

        try
        {
            await _mediator.Send(command);
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
