using System.Threading.Tasks;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IUnitOfWork
{
    Task CompleteAsync();
}
