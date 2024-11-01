using MediatR;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

public record GetUserByUserNameQuery(string UserName):IRequest<User?>;
