using System;
using System.Linq;
using System.Linq.Expressions;

namespace ProjectMetadataPlatform.Application.Interfaces;

public interface IRepositoryBase<T>
{
    IQueryable<T> GetEverything();
    IQueryable<T> GetIf(Expression<Func<T, bool>> expression);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}