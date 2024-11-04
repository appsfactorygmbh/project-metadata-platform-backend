using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;


/// <summary>
/// Handles the command to delete a user by their unique identifier.
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand,User?>
{
    private readonly IUsersRepository _usersRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteUserCommandHandler"/> class.
    /// </summary>
    /// <param name="usersRepository">The users repository.</param>
    public DeleteUserCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
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

        if (user == null)
        {
            return null;
        }

        return await _usersRepository.DeleteUserAsync(user);

    }
}
