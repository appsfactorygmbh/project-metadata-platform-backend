using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;
using Microsoft.AspNetCore.Http;



namespace ProjectMetadataPlatform.Application.Users;


/// <summary>
/// Handles the command to delete a user by their unique identifier.
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand,User?>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteUserCommandHandler"/> class.
    /// </summary>
    /// <param name="usersRepository">The users repository.</param>
    /// <param name="httpContextAccessor">Provides Access to the current Http Context.</param>
    public DeleteUserCommandHandler(IUsersRepository usersRepository, IHttpContextAccessor httpContextAccessor)
    {
        _usersRepository = usersRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Handles the <see cref="DeleteUserCommand"/> request.
    /// </summary>
    /// <param name="request">The command request containing the user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns> Deletes User if present, otherwise null.</returns>
    public async Task<User?> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetUserByIdAsync(request.Id);
        var username = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "Unknown user";
        User? activeUser = await _usersRepository.GetUserByUserNameAsync(username);
        return user !=null && user == activeUser
            ? throw new InvalidOperationException("A User can't delete themself.")
            : user == null ? null :await _usersRepository.DeleteUserAsync(user);
    }
}
