using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Teams;

/// <summary>
/// The repository for users that handles the data access.
/// </summary>
public class TeamRepository : RepositoryBase<Team>, ITeamRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TeamRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    public TeamRepository(ProjectMetadataPlatformDbContext dbContext)
        : base(dbContext)
    {
        _context = dbContext;
    }

    /// <inheritdoc/>
    public Task<bool> CheckIfTeamExists(int id)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc/>
    public Task<string> RetrieveNameForId(int id)
    {
        throw new System.NotImplementedException();
    }
}
