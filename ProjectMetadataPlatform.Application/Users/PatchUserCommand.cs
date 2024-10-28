using MediatR;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

public record PatchUserCommand(int Id, string? Username = null, string? Name = null, string? Email = null, string? Password = null): IRequest<User?>;
