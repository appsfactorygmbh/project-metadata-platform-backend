using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ProjectMetadataPlatform.Application.Users;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
{

    public Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}
