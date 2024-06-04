namespace ProjectMetadataPlatform.Api.Projects.Models;

public record GetProjectResponse(string ProjectName, string ClientName, string BusinessUnit, int TeamNumber, string Department)
{
    
}