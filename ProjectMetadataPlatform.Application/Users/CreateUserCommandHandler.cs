using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Logs;
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
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;


    /// <summary>
    ///    Creates a new instance of <see cref="CreateUserCommandHandler" />.
    /// </summary>
    /// <param name="usersRepository">Repository for accessing user data.</param>
    /// <param name="logRepository">Repository for logging data.</param>
    /// <param name="unitOfWork">Unit of work for managing transactions.</param>
    public CreateUserCommandHandler(IUsersRepository usersRepository,ILogRepository logRepository, IUnitOfWork unitOfWork)
    {
        _usersRepository = usersRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }


    /// <summary>
    ///    Creates a new User with the given data.
    /// </summary>
    /// <param name="request">Request for user creation.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The ID of the created user.</returns>
    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User { Name = request.Name, UserName = request.Username, Email = request.Email };
        var result = await _usersRepository.CreateUserAsync(user, request.Password);

        var changes = new List<LogChange>
        {
            new() { OldValue = "", NewValue = user.Email, Property = nameof(User.Email) }
        };
        await _logRepository.AddUserLogForCurrentUser(user, Action.ADDED_USER, changes);

        await _unitOfWork.CompleteAsync();
}
