using System.Linq.Expressions;
using Common.Entities;

namespace Common.Repository
{
    public interface IRepository<T> where T : IEntity
    {
        /// <summary>
        /// Gets all entities from collection
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets filtered entities from collection
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T,bool>> filter, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets entity from collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEntity> GetAsync(Guid id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets enitity by filter from collection
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEntity> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
        /// <summary>
        /// Creates and adds new entity to collection
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task CreateAsync(T entity, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes entity from collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates entity in collection
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    }
}
