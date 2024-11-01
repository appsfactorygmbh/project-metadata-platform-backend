using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

public class GetUserByUserNameQueryHandler: IRequestHandler<GetUserByUserNameQuery, User?>
{
    private readonly IUsersRepository _usersRepository;

    public GetUserByUserNameQueryHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public Task<User?> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
    {
        return _usersRepository.GetUserByUserNameAsync(request.UserName);
    }
}
