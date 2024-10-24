using MediatR;

namespace ProjectMetadataPlatform.Application.Users;

public record CreateUserCommand(int UserId, string Username, string Name, string Email, string Password):IRequest;
