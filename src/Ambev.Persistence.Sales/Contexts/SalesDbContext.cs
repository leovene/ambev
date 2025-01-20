using Ambev.Domain.Sales.Entities;
using Ambev.Persistence.Common.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Ambev.Persistence.Sales.Contexts
{
    public class SalesDbContext : AppDbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

        public DbSet<SaleEntity> Sales { get; set; }
        public DbSet<SaleItemEntity> SaleItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SaleEntity>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(s => s.SaleNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(s => s.CustomerId)
                    .IsRequired();

                entity.Property(s => s.Branch)
                    .HasMaxLength(100);

                entity.Property(s => s.SaleDate)
                    .IsRequired();

                entity.Property(s => s.IsCancelled)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Ignore(s => s.TotalAmount); 

                entity.HasMany(s => s.Items)
                    .WithOne()
                    .HasForeignKey(i => i.SaleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(s => s.IsSynced)
                    .IsRequired()
                    .HasDefaultValue(false);
            });

            modelBuilder.Entity<SaleItemEntity>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(i => i.SaleId)
                    .IsRequired();

                entity.Property(i => i.ProductId)
                    .IsRequired();

                entity.Property(i => i.Quantity)
                    .IsRequired();

                entity.Property(i => i.UnitPrice)
                    .IsRequired()
                    .HasColumnType("numeric(18,2)");

                entity.Property(i => i.Discount)
                    .IsRequired()
                    .HasColumnType("numeric(5,4)");

                entity.Property(i => i.IsCancelled)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Ignore(i => i.TotalAmount);
            });
        }
    }
}
