using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Users;

public class PatchUserCommandHandler: IRequestHandler<PatchUserCommand, IdentityUser?>
{
    private readonly IUsersRepository _usersRepository;

    public PatchUserCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<IdentityUser?> Handle(PatchUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetUserByIdAsync(request.Id);

        if (user == null)
        {
            return null;
        }

        user.UserName = request.Username ?? user.UserName;
        // user.Name = request.Name ?? user.Name;
        user.Email = request.Email ?? user.Email;
        // todo
        user.PasswordHash = request.Password ?? user.PasswordHash;

        return await _usersRepository.StoreUser(user);
    }
}
