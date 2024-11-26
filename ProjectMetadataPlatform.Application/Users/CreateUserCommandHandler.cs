using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// /// <summary>
///     Handler for the <see cref="CreateUserCommand" />
/// </summary>
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
{
    private readonly IUsersRepository _usersRepository;

    /// <summary>
    ///    Creates a new instance of<see cref="CreateUserCommandHandler" />.
    /// </summary>
    /// <param name="usersRepository">Repository for accessing user data.</param>
    public CreateUserCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }


    /// <summary>
    ///    Creates a new User with the given data.
    /// </summary>
    /// <param name="request">Request for user creation. </param>
    /// <param name="cancellationToken"></param>
    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User { Email = request.Email };
        var result = await _usersRepository.CreateUserAsync(user, request.Password);
        return result;
    }
}
