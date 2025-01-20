using Ambev.Domain.Sales.Entities;

namespace Ambev.Domain.Sales.Interfaces.Repositories
{
    public interface ISaleDocumentRepository
    {
        public Task AddSaleAsync(SaleDocumentEntity saleDocument);
        public Task<List<SaleDocumentEntity>> GetAllSalesAsync();
    }
}
