using System;
using System.Linq;
using System.Linq.Expressions;
using ProjectMetadataPlatform.Application.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected ProjectMetadataPlatformDbContext ProjectMetadataPlatformDbContext;
    
    public RepositoryBase(ProjectMetadataPlatformDbContext _ProjectMetadataPlatformDbContext)
    {
        ProjectMetadataPlatformDbContext = _ProjectMetadataPlatformDbContext;
    }

    public IQueryable<T> GetEverything() =>
        ProjectMetadataPlatformDbContext.Set<T>()
            .AsNoTracking();

    public IQueryable<T> GetIf(Expression<Func<T, bool>> expression) =>
        ProjectMetadataPlatformDbContext.Set<T>()
            .Where(expression)
            .AsNoTracking();

    public void Create(T entity) => ProjectMetadataPlatformDbContext.Set<T>().Add(entity);

    public void Update(T entity) => ProjectMetadataPlatformDbContext.Set<T>().Update(entity);

    public void Delete(T entity) => ProjectMetadataPlatformDbContext.Set<T>().Remove(entity);
}
