using System.Linq.Expressions;

namespace Ambev.Domain.Common.Interfaces.Repositories
{
    public interface IQueryRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(string tableName, Guid id);
        Task<List<T>> GetAllAsync(string tableName);
    }
}
