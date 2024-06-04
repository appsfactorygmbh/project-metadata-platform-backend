namespace ProjectMetadataPlatform.Api.Projects.Models;

public record GetProjectResponse(int id, string ProjectName, string ClientName, string BusinessUnit, int TeamNumber, string Department)
{
    
}