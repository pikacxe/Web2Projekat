using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Common.Entities;

namespace Common.Repository
{
    public interface IRepository<T> where T : IEntity
    {
        /// <summary>
        /// Gets all entities from collection
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyCollection<T>> GetAllAsync();
        /// <summary>
        /// Gets filtered entities from collection
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T,bool>> filter);
        /// <summary>
        /// Gets entity from collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEntity> GetAsync(Guid id);
        /// <summary>
        /// Gets enitity by filter from collection
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEntity> GetAsync(Expression<Func<T, bool>> filter);
        /// <summary>
        /// Creates and adds new entity to collection
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task CreateAsync(T entity);
        /// <summary>
        /// Deletes entity from collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);
        /// <summary>
        /// Updates entity in collection
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(T entity);
    }
}
