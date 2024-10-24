using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly ProjectMetadataPlatformDbContext _context;
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectsRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    public UserRepository(ProjectMetadataPlatformDbContext dbContext,UserManager<User> userManager) : base(dbContext)
    {
        _userManager = userManager;
        _context = dbContext;
    }

    public async Task CreateUserAsync(User user)
    {
        if (_userManager.FindByIdAsync(user.Id) == null)
        {

            _context.Users.Add(user);
        }
        _ = await _context.SaveChangesAsync();

    }
}
