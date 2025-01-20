using Ambev.Domain.Sales.Entities;
using Ambev.Domain.Sales.Interfaces.Repositories;
using Ambev.Persistence.Common.Commands;
using Ambev.Persistence.Common.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Ambev.Persistence.Sales.Commands
{
    public class SaleCommandRepository : CommandRepository<SaleEntity>, ISaleCommandRepository
    {
        public SaleCommandRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task UpdateSaleAsync(SaleEntity sale)
        {
            var trackedEntity = await _dbSet.Include(e => e.Items).FirstOrDefaultAsync(e => e.Id == sale.Id);

            if (trackedEntity == null)
            {
                throw new KeyNotFoundException("Entity not found in the database.");
            }

            _context.Entry(trackedEntity).CurrentValues.SetValues(sale);

            foreach (var item in sale.Items)
            {
                var trackedItem = trackedEntity.Items.FirstOrDefault(i => i.Id == item.Id);

                if (trackedItem != null)
                {
                    _context.Entry(trackedItem).CurrentValues.SetValues(item);
                }
                else
                {
                    trackedEntity.Items.Add(item);
                }
            }

            var itemsToRemove = trackedEntity.Items
                .Where(existingItem => sale.Items.All(i => i.Id != existingItem.Id))
                .ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                _context.Remove(itemToRemove);
            }

            await _context.SaveChangesAsync();
        }
    }
}
