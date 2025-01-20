using Ambev.Domain.Common.Interfaces.Repositories;
using Ambev.Persistence.Common.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Ambev.Persistence.Common.Commands
{
    public abstract class CommandRepository<T> : ICommandRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected CommandRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            var existingEntry = _context.Entry(entity);
            if (existingEntry.State != EntityState.Detached)
            {
                _dbSet.Remove(entity);
            }
            else
            {
                var trackedEntity = await _dbSet.FindAsync(GetKeyValues(entity));
                if (trackedEntity == null)
                {
                    throw new KeyNotFoundException($"Entity of type {typeof(T).Name} not found.");
                }

                _dbSet.Remove(trackedEntity);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            var trackedEntity = await _dbSet.FindAsync(GetKeyValues(entity));

            if (trackedEntity == null)
            {
                throw new KeyNotFoundException("Entity not found in the database.");
            }

            _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
        }

        private object?[] GetKeyValues(T entity)
        {
            var keyProperties = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties;
            if (keyProperties == null)
            {
                throw new InvalidOperationException($"Entity {typeof(T).Name} does not have a primary key defined.");
            }

            return keyProperties.Select(p => p.PropertyInfo?.GetValue(entity)).ToArray();
        }
    }
}
