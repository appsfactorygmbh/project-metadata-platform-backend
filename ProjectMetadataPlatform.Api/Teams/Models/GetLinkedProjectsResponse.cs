using System.Collections.Generic;

namespace ProjectMetadataPlatform.Api.Teams.Models;

/// <summary>
/// Response for receiving the linked projects for a team.
/// </summary>
/// <param name="ProjectIds">A list of project ids the team is linked to.</param>
public record GetLinkedProjectsResponse(List<int> ProjectIds);
