using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.User;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IUserRepository
{
    Task CreateUserAsync(User user);
}
