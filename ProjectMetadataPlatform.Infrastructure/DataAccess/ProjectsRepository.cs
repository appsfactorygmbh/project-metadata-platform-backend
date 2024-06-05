using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

public class ProjectsRepository : RepositoryBase<Project>, IProjectsRepository
{
    public ProjectsRepository(ProjectMetadataPlatformDbContext ProjectMetadataPlatformDbContext) : base(
        ProjectMetadataPlatformDbContext)
    {
    }
    public async Task<IEnumerable<Project>> GetAllProjectsAsync() => 
        await GetEverything().ToListAsync();
}
