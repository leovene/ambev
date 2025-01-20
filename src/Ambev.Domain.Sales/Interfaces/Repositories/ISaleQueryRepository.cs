using Ambev.Domain.Common.Interfaces.Repositories;
using Ambev.Domain.Sales.Entities;

namespace Ambev.Domain.Sales.Interfaces.Repositories
{
    public interface ISaleQueryRepository : IQueryRepository<SaleEntity>
    {
        public Task<List<SaleEntity>> GetAllWithItemsAsync();
        public Task<SaleEntity?> GetByIdWithItemsAsync(Guid id);
        public Task<IEnumerable<SaleEntity>> GetUnsyncedSalesAsync();
    }
}
