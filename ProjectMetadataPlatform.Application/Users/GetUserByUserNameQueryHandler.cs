using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Handles the query to get a user by their username.
/// </summary>
public class GetUserByUserNameQueryHandler: IRequestHandler<GetUserByUserNameQuery, User?>
{
    private readonly IUsersRepository _usersRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByUserNameQueryHandler"/> class.
    /// </summary>
    /// <param name="usersRepository">The repository for accessing user data.</param>
    public GetUserByUserNameQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    /// <summary>
    /// Handles the query to get a user by their username.
    /// </summary>
    /// <param name="request">The query request containing the username.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The user with the specified username, or null if not found.</returns>
    public Task<User?> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
    {
        return _usersRepository.GetUserByUserNameAsync(request.UserName);
    }
}
