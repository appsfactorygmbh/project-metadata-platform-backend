using System;
using System.Linq;
using System.Linq.Expressions;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Provides a base repository interface for CRUD operations and querying entities.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public interface IRepositoryBase<T>
{
    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/> as an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <returns>An <see cref="IQueryable{T}"/> containing all entities.</returns>
    IQueryable<T> GetEverything();
    
    /// <summary>
    /// Retrieves entities of type <typeparamref name="T"/> that satisfy the specified condition.
    /// </summary>
    /// <param name="expression">An <see cref="Expression{Func{T, bool}}"/> to filter the entities.</param>
    /// <returns>An <see cref="IQueryable{T}"/> containing the entities that match the condition.</returns>
    IQueryable<T> GetIf(Expression<Func<T, bool>> expression);
    
    /// <summary>
    /// Adds a new entity of type <typeparamref name="T"/> to the repository.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    void Create(T entity);
    
    /// <summary>
    /// Updates an existing entity of type <typeparamref name="T"/> in the repository.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    void Update(T entity);
    
    /// <summary>
    /// Deletes an existing entity of type <typeparamref name="T"/> from the repository.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    void Delete(T entity);
}