using Ambev.Domain.Common.Interfaces.Repositories;
using Dapper;
using System.Data;

namespace Ambev.Persistence.Common.Queries
{
    public abstract class QueryRepository<T> : IQueryRepository<T> where T : class
    {
        protected readonly IDbConnection _dbConnection;

        protected QueryRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<T>> GetAllAsync(string tableName)
        {
            var sql = $"SELECT * FROM \"{tableName}\"";
            return (await _dbConnection.QueryAsync<T>(sql)).ToList();
        }

        public async Task<T?> GetByIdAsync(string tableName, Guid id)
        {
            var sql = $"SELECT * FROM \"{tableName}\" WHERE \"Id\" = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
        }
    }
}
