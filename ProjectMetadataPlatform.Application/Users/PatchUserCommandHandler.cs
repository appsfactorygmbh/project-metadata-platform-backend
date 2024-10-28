using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

public class PatchUserCommandHandler: IRequestHandler<PatchUserCommand, User?>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public PatchUserCommandHandler(IUsersRepository usersRepository, IPasswordHasher<User> passwordHasher)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> Handle(PatchUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetUserByIdAsync(request.Id);

        if (user == null)
        {
            return null;
        }

        user.UserName = request.Username ?? user.UserName;
        user.Name = request.Name ?? user.Name;
        user.Email = request.Email ?? user.Email;
        user.PasswordHash = request.Password != null ? _passwordHasher.HashPassword(user, request.Password) : user.PasswordHash;

        return await _usersRepository.StoreUser(user);
    }
}
