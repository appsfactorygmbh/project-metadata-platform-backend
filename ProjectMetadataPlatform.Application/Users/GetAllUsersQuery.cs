using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
///     Query to retrieve all projects.
/// </summary>
public record GetAllUsersQuery() : IRequest<IEnumerable<User>>;
