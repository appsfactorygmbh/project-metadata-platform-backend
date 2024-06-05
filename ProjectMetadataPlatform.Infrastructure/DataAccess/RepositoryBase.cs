using System;
using System.Linq;
using System.Linq.Expressions;
using ProjectMetadataPlatform.Application.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace ProjectMetadataPlatform.Infrastructure.DataAccess;

/// <summary>
/// Base repository class providing common data access methods for entities.
/// </summary>
/// <typeparam name="T">The type of entity being managed.</typeparam>
public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    /// <summary>
    /// The database context used for data access.
    /// </summary>
    private ProjectMetadataPlatformDbContext ProjectMetadataPlatformDbContext;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryBase{T}"/> class.
    /// </summary>
    /// <param name="_ProjectMetadataPlatformDbContext">The database context.</param>
    public RepositoryBase(ProjectMetadataPlatformDbContext _ProjectMetadataPlatformDbContext)
    {
        ProjectMetadataPlatformDbContext = _ProjectMetadataPlatformDbContext;
    }

    /// <summary>
    /// Gets all entities of type <typeparamref name="T"/> from the database without tracking changes.
    /// </summary>
    /// <returns>An <see cref="IQueryable{T}"/> of all entities.</returns>
    public IQueryable<T> GetEverything() =>
        ProjectMetadataPlatformDbContext.Set<T>()
            .AsNoTracking();

    /// <summary>
    /// Gets entities of type <typeparamref name="T"/> from the database that satisfy the specified condition without tracking changes.
    /// </summary>
    /// <param name="expression">The condition to filter the entities.</param>
    /// <returns>An <see cref="IQueryable{T}"/> of entities that satisfy the condition.</returns>
    public IQueryable<T> GetIf(Expression<Func<T, bool>> expression) =>
        ProjectMetadataPlatformDbContext.Set<T>()
            .Where(expression)
            .AsNoTracking();

    /// <summary>
    /// Adds a new entity of type <typeparamref name="T"/> to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public void Create(T entity) => ProjectMetadataPlatformDbContext.Set<T>().Add(entity);

    /// <summary>
    /// Updates an existing entity of type <typeparamref name="T"/> in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public void Update(T entity) => ProjectMetadataPlatformDbContext.Set<T>().Update(entity);

    /// <summary>
    /// Deletes an existing entity of type <typeparamref name="T"/> from the database.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public void Delete(T entity) => ProjectMetadataPlatformDbContext.Set<T>().Remove(entity);
}
