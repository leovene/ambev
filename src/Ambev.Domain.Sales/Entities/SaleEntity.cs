namespace Ambev.Domain.Sales.Entities
{
    public class SaleEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required Guid CustomerId { get; set; }
        public string SaleNumber { get; set; } = Guid.NewGuid().ToString();
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public string? Branch { get; set; }
        public decimal TotalAmount => Items.Sum(i => i.TotalAmount);
        public bool IsCancelled { get; set; } = false;
        public List<SaleItemEntity> Items { get; private set; } = new();
        public bool IsSynced { get; set; } = false;

        public void AddItem(SaleItemEntity item)
        {
            if (Items.Any(i => i.ProductId == item.ProductId))
            {
                throw new InvalidOperationException("Item already exists in the sale.");
            }

            Items.Add(item);
        }
    }
}
