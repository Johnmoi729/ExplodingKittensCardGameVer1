using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExplodingKittens.Application.Interfaces
{
    /// <summary>
    /// Generic repository interface for CRUD operations
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(string id);
        Task<T> AddAsync(T entity);
        Task<bool> UpdateAsync(string id, T entity);
        Task<bool> DeleteAsync(string id);
    }
}