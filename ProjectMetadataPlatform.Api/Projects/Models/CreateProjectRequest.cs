using System.Collections.Generic;
using ProjectMetadataPlatform.Api.Plugins.Models;

namespace ProjectMetadataPlatform.Api.Projects.Models;

/// <summary>
/// Represents a request to create a new project.
/// </summary>
/// <param name="ProjectName">The name of the project.</param>
/// <param name="ClientName">The name of the client for the project.</param>
/// <param name="BusinessUnit">The name of the Business Unit associated with the project.</param>
/// <param name="TeamNumber">The number of the team working on the project.</param>
/// <param name="Department">The name of the department associated with the project.</param>
/// <param name="PluginList">An optional list of plugins associated to the project.</param>
public record CreateProjectRequest(string ProjectName, string BusinessUnit, int TeamNumber, string Department, string ClientName, List<GetPluginResponse>? PluginList = null);
