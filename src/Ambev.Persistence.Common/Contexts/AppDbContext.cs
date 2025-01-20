using Microsoft.EntityFrameworkCore;

namespace Ambev.Persistence.Common.Contexts
{
    public abstract class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
    }
}
