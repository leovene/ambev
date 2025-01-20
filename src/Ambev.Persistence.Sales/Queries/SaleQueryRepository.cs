using Ambev.Domain.Sales.Entities;
using Ambev.Domain.Sales.Interfaces.Repositories;
using Ambev.Persistence.Common.Queries;
using Dapper;
using System.Data;

namespace Ambev.Persistence.Sales.Queries
{
    public class SaleQueryRepository : QueryRepository<SaleEntity>, ISaleQueryRepository
    {
        public SaleQueryRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public async Task<List<SaleEntity>> GetAllWithItemsAsync()
        {
            var sql = @"SELECT s.*, si.* FROM ""Sales"" s
                LEFT JOIN ""SaleItems"" si ON si.""SaleId"" = s.""Id""";

            var salesDictionary = new Dictionary<Guid, SaleEntity>();

            var result = await _dbConnection.QueryAsync<SaleEntity, SaleItemEntity, SaleEntity>(
                sql,
                (sale, item) =>
                {
                    if (!salesDictionary.TryGetValue(sale.Id, out var currentSale))
                    {
                        currentSale = sale;
                        salesDictionary.Add(sale.Id, currentSale);
                    }

                    if (item != null)
                    {
                        currentSale.Items.Add(item);
                    }

                    return currentSale;
                }
            );

            return salesDictionary.Values.ToList();
        }


        public async Task<SaleEntity?> GetByIdWithItemsAsync(Guid id)
        {
            var sql = @"SELECT s.*, si.* FROM ""Sales"" s
                        LEFT JOIN ""SaleItems"" si ON si.""SaleId"" = s.""Id""
                        WHERE s.""Id"" = @Id";

            var salesDictionary = new Dictionary<Guid, SaleEntity>();

            var result = await _dbConnection.QueryAsync<SaleEntity, SaleItemEntity, SaleEntity>(
                sql,
                (sale, item) =>
                {
                    if (!salesDictionary.TryGetValue(sale.Id, out var currentSale))
                    {
                        currentSale = sale;
                        salesDictionary.Add(sale.Id, currentSale);
                    }

                    if (item != null)
                    {
                        currentSale.AddItem(item);
                    }

                    return currentSale;
                },
                new { Id = id }
            );

            return salesDictionary.Values.FirstOrDefault();
        }

        public async Task<IEnumerable<SaleEntity>> GetUnsyncedSalesAsync()
        {
            var sql = @"SELECT s.*, si.* 
                FROM ""Sales"" s
                LEFT JOIN ""SaleItems"" si ON si.""SaleId"" = s.""Id""
                WHERE s.""IsSynced"" = false";

            var salesDictionary = new Dictionary<Guid, SaleEntity>();

            var result = await _dbConnection.QueryAsync<SaleEntity, SaleItemEntity, SaleEntity>(
                sql,
                (sale, item) =>
                {
                    if (!salesDictionary.TryGetValue(sale.Id, out var currentSale))
                    {
                        currentSale = sale;
                        salesDictionary.Add(sale.Id, currentSale);
                    }

                    if (item != null)
                    {
                        currentSale.Items.Add(item);
                    }

                    return currentSale;
                }
            );

            return salesDictionary.Values;
        }
    }
}
