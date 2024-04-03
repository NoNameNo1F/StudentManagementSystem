using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);
        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true);
        Task CreateAsync(T entity);
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
