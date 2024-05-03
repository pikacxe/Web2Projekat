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
        Task<IReadOnlyCollection<T>> GetAllAsync();
        Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T,bool>> filter);
        Task<IEntity> GetAsync(Guid id);
        Task<IEntity> GetAsync(Expression<Func<T, bool>> filter);
        Task CreateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task UpdateAsync(T entity);
    }
}
