using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Infrastructure.Users;

/// <summary>
///     The repository for users that handles the data access.
/// </summary>
public class UsersRepository : RepositoryBase<User>, IUsersRepository
{
    private readonly UserManager<User> _userManager;
    private readonly ProjectMetadataPlatformDbContext _context;
    /// <summary>
    ///     Initializes a new instance of the <see cref="UsersRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    /// <param name="userManager">Manager for users of the type user.</param>
    public UsersRepository(ProjectMetadataPlatformDbContext dbContext,UserManager<User> userManager) : base(dbContext)
    {
        _userManager = userManager;
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
