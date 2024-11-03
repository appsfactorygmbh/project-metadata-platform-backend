using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand,User?>
{
    private readonly IUsersRepository _usersRepository;

    public DeleteUserCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

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
