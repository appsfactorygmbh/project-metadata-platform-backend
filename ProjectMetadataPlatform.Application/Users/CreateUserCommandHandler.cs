using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// /// <summary>
///     Handler for the <see cref="CreateUserCommand" />
/// </summary>
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
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
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (_usersRepository.GetUserByIdAsync(request.UserId).Result!= null)
        {
            throw new ArgumentException("User with this Id already exists.");
        }
        var user = new User { Name = request.Name, UserName = request.Username, Email = request.Email, Id = request.UserId.ToString()};
        await _usersRepository.CreateUserAsync(user, request.Password);




    }
}
