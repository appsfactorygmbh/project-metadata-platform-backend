namespace ProjectMetadataPlatform.Api.Projects.Models;

public record GetProjectsResponse(string ProjectName, string ClientName, string BusinessUnit, int TeamNumber)
{
    
}