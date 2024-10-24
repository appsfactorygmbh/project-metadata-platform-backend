using System.Linq;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    private readonly ProjectMetadataPlatformDbContext _context;
    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectsRepository" /> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing project data.</param>
    public UserRepository(ProjectMetadataPlatformDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task CreateUserAsync(User user)
    {
        if (GetIf(p => p.Id == user.Id).FirstOrDefault() == null)
        {
            Create(user);
        }
        _ = await _context.SaveChangesAsync();
    }
}
