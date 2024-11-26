using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Handles the command to patch user information.
/// </summary>
public class PatchUserCommandHandler : IRequestHandler<PatchUserCommand, User?>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatchUserCommandHandler"/> class.
    /// </summary>
    /// <param name="usersRepository">The repository for accessing user data.</param>
    /// <param name="passwordHasher">The service for hashing user passwords.</param>
    public PatchUserCommandHandler(IUsersRepository usersRepository, IPasswordHasher<User> passwordHasher)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Handles the patch user command.
    /// </summary>
    /// <param name="request">The command containing the user information to be patched.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated user information, or null if the user was not found.</returns>
    public async Task<User?> Handle(PatchUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetUserByIdAsync(request.Id);

        if (user == null)
        {
            return null;
        }

        user.Email = request.Email ?? user.Email;
        user.PasswordHash = request.Password != null ? _passwordHasher.HashPassword(user, request.Password) : user.PasswordHash;

        return await _usersRepository.StoreUser(user);
    }
}
