namespace Ambev.Domain.Sales.Entities
{
    public class SaleItemDocumentEntity
    {
        public string? Id { get; set; }
        public string? SaleId { get; set; }
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount => Quantity * UnitPrice * (1 - Discount);
    }
}
