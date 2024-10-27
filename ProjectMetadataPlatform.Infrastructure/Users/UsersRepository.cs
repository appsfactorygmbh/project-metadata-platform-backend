using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Users;

/// <summary>
///     The repository for users that handles the data access.
/// </summary>
public class UsersRepository : RepositoryBase<User>, IUsersRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;
    /// <summary>
    ///     Initializes a new instance of the <see cref="UsersRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    public UsersRepository(ProjectMetadataPlatformDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    /// <summary>
    ///     Asynchronously retrieves all projects from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. When this task completes, it returns a collection of projects.</returns>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await GetEverything().ToListAsync();
    }
}
