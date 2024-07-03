using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Plugins;


namespace ProjectMetadataPlatform.Application.Projects;
/// <summary>
/// Command to update a project with the given attributes.
/// </summary>
/// <param name="ProjectName">Name of the project</param>
/// <param name="BusinessUnit">Name of the business unit</param>
/// <param name="TeamNumber">Number of the team</param>
/// <param name="Department">Name of the department</param>
/// <param name="ClientName">Name of the client</param>
/// <param name="Id">Id of the project</param>
/// <param name="Plugins">List of plugins associated with the project</param>
public record UpdateProjectCommand(string ProjectName, string BusinessUnit, int TeamNumber, string Department, string ClientName, int Id, List<ProjectPlugins> Plugins): IRequest<int>;
