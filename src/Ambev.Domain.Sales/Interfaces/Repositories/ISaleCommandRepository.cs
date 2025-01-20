using Ambev.Domain.Common.Interfaces.Repositories;
using Ambev.Domain.Sales.Entities;

namespace Ambev.Domain.Sales.Interfaces.Repositories
{
    public interface ISaleCommandRepository : ICommandRepository<SaleEntity>
    {
        public Task UpdateSaleAsync(SaleEntity sale);
    }
}
