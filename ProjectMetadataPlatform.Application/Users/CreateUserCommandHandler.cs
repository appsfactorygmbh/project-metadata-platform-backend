using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {

        var user = new User { Name = request.Name, Email = request.Email, Id = request.UserId.ToString() };

        await _userRepository.CreateUserAsync(user);



    }
}
