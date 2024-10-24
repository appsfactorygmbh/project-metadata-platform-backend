using MediatR;

namespace ProjectMetadataPlatform.Application.Users;

public record CreateUserCommand(string UserId, string Username, string Name, string Email, string Password):IRequest;
