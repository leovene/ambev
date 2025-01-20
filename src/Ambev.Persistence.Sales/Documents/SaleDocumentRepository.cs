using Ambev.Domain.Sales.Entities;
using Ambev.Domain.Sales.Interfaces.Repositories;
using MongoDB.Driver;

namespace Ambev.Persistence.Sales.Documents
{
    public class SaleDocumentRepository : ISaleDocumentRepository
    {
        private readonly IMongoCollection<SaleDocumentEntity> _salesCollection;

        public SaleDocumentRepository(IMongoDatabase database)
        {
            _salesCollection = database.GetCollection<SaleDocumentEntity>("Sales");
        }

        public async Task AddSaleAsync(SaleDocumentEntity saleDocument)
        {
            await _salesCollection.InsertOneAsync(saleDocument);
        }

        public async Task<List<SaleDocumentEntity>> GetAllSalesAsync()
        {
            return await _salesCollection.Find(_ => true).ToListAsync();
        }
    }
}
