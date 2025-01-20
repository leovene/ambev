namespace Ambev.Domain.Sales.Entities
{
    public class SaleDocumentEntity
    {
        public string? Id { get; set; }
        public string? CustomerId { get; set; }
        public string? SaleNumber { get; set; }
        public DateTime? SaleDate { get; set; }
        public string? Branch { get; set; }
        public decimal TotalAmount => Items.Sum(i => i.TotalAmount);
        public List<SaleItemDocumentEntity> Items { get; private set; } = new();
    }
}
